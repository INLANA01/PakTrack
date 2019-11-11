using PakTrack.Models.Sensor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using PakTrack.Utilities;
using System.IO;
using System.Numerics;
using PakTrack.Core;
using PakTrack.DAL.Interfaces.Sensor;

namespace PakTrack.DAL.Context
{
    public class SensorReaderContext: ISensorReaderContext
    {
        //Serial Port to communicate with sensor
        private  SerialPort _port;
        //Sensor writer class to write At commands to sensor
        private  SensorWriterContext _sensorWriter;
        private readonly ConcurrentDictionary<string, List<Packet>> _partialData;

        //Delegate to read data from sensor or input file. 
        private delegate byte BinaryReaderHelper();
        //truck id
        private string _truckId;
        private string _packageId;
        private string _sensorId;
        private string _notes;
        private bool _isFashAvailable;
        private int _totalPackets;
        private int _readPackets;
        private int MAXTIMEOUTTRIES = 5;

//
//        public event ApplicationEvent.NotifyPacketCreated PacketCreatedEvent;
//        public event ApplicationEvent.DataReadComplete SensorDataReadCompletEvent;
        private BufferedStream _inputPortStreamReader;

        public int GetTotalPacketsLength() => _totalPackets;


        public int GetTotalReadPacketsLength() => _readPackets;

        private  readonly DateTime Jan1st1970 = new DateTime
            (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private  long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
        public SensorReaderContext()
        {
            _partialData = new ConcurrentDictionary<string, List<Packet>>();
        }


        public string TruckId() => _truckId;

        public string PackageId() => _packageId;

        public string SensorId() => _sensorId;

        public string Notes() => _notes;

        public void Connect(string port)
        {
            if (_port == null || !_port.PortName.Equals(port))
            {
                _port = new SerialPort(port)
                {
                    BaudRate = 230400,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ReadBufferSize = 4096
                };
            }
            if (!_port.IsOpen)
            {
                _port.Open();
                _inputPortStreamReader = new BufferedStream(_port.BaseStream);
            }
            if (_port == null || !_port.IsOpen) return;
            _sensorWriter = new SensorWriterContext(_port);
            _sensorId = GetBoardId();
            _notes = GetNote();
            var noteArray = _notes.Split('-');
            if (noteArray.Length > 1)
            {
                _truckId = noteArray[0];
                _packageId = noteArray[1];
            }
        }

        public void Disconneect()
        {
            if (_port.IsOpen)
            {
                _port.Close();
                _inputPortStreamReader.Close();
                _inputPortStreamReader = null;
            }
        }

        /// <summary>
        /// Read the sync frame from files and ports
        /// </summary>
        /// <param name="reader">Source from where the sync frame should be read</param>
        /// <param name="syncFrame"></param>
        /// <returns>Boolen, true if read sucessfully else false</returns>
        private bool ReadSyncFrame(BinaryReaderHelper reader, out byte[] syncFrame)
        {
            var syncFrameByte = 0;
            var notInSyncCount = 0;
            var syncFrameIndex = 0;
            syncFrame = new byte[SensorConstants.FRAME_SYNC.Length];
            //Loop till we find first byte of the SyncFrame
            while (syncFrameByte != 170) //170 = OxAA
            {
                try
                {
                    syncFrameByte = reader() & 0xff;
                    notInSyncCount++;
                    if (notInSyncCount > SensorConstants.MTU)
                    {
                        return false;
                    }
                }
                catch (TimeoutException)
                {
                    return false;
                }
                catch (EndOfStreamException)
                {
                    return false;
                }

            }

            if (syncFrameByte != 170) return false;
            syncFrame[syncFrameIndex++] = (byte)(syncFrameByte & 0xff);

            while (syncFrameIndex < SensorConstants.FRAME_SYNC.Length && notInSyncCount < SensorConstants.MTU)
            {
                syncFrameByte = reader();
                syncFrame[syncFrameIndex++] = (byte)(syncFrameByte & 0xff);
                notInSyncCount++;
            }

            return Compare(syncFrame, SensorConstants.FRAME_SYNC) && notInSyncCount < SensorConstants.MTU;
        }

        /// <summary>
        /// Compare two byte arrays
        /// </summary>
        /// <param name="first">byte array one</param>
        /// <param name="second">byte array two</param>
        /// <returns>true if the arrays have same contents else false</returns>
        private static bool Compare(byte[] first, int[] second) // from Utils.Compare
        {
            var isEqual = true;
            if (first.Length != second.Length) return false;
            if (second.Where((t, i) => (first[i] & 0xff) != t).Any())
                isEqual = false;
            return isEqual;
        }


        /// <summary>
        /// Read a packet frame from the sensor or filestream if provided
        /// </summary>
        /// <param name="readSdCard"></param>
        /// <param name="input">optional parameter filestream</param>
        /// <returns>A packet frame byte array representations</returns>
        public byte[] ReadFramedPacket(bool readSdCard, BinaryReader input = null)
        {
            BinaryReaderHelper myHelper = () => ReadByte(_port);
            myHelper = input != null ? () => ReadByte(input) : myHelper;
            byte[] receiveBuffer;
            bool readSucessfuly;
            try
            {
                readSucessfuly = ReadPacket(readSdCard, myHelper, out receiveBuffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return readSucessfuly ? receiveBuffer :null;
        }

        private byte ReadByte(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        private byte ReadByte(SerialPort reader)
        {
            var counter = 0;
            var tempData = _inputPortStreamReader.ReadByte();
            while (tempData == -1)
            {
                try
                {
                    System.Threading.Thread.Sleep(10);
                    counter++;
                    if (counter > 20)
                    {
                        return unchecked((byte)-1);
                    }
                    tempData = (byte)_inputPortStreamReader.ReadByte();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _inputPortStreamReader.Flush();
            return (byte)(tempData & 0xff);
        }
        /// <summary>
        /// Read Meta data from the Sensor 
        /// Meta data includes Command, NodeId, Length
        /// </summary>
        /// <param name="reader"> Input Helper</param>
        /// <param name="buffer">Packet with metadata read into it</param>
        /// <returns>Read pointer stating how many bytes read</returns>
        private int ReadMeta(BinaryReaderHelper reader, out byte[] buffer)
        {
            var readPointer = 0;
            buffer = new byte[SensorConstants.MTU];

            // Read request/response Command 1 Byte Always D1
            // This commaond diffrentiates the request packet from 
            // resposne packet
            var command = reader();
            buffer[readPointer++] = command;
            //Read node Id 2 bytes
            buffer[readPointer++] = reader();
            buffer[readPointer++] = reader();
            //Read Length 2 Bytes
            buffer[readPointer++] = reader();
            buffer[readPointer++] = reader();

            return readPointer;
        }


        /// <summary>
        /// Read the frame header and extract the payload payLoadLength
        /// </summary>
        /// <param name="buffer">Packet header</param>
        /// <param name="count">current read counter</param>
        /// <param name="readFromSd"></param>
        /// <returns>Lenght of thte payload to read</returns>
        private int CalculateLength(byte[] buffer, int count, bool readFromSd)
        {
            if (!readFromSd)
                return (buffer[count - 1] & 0xff) | (buffer[count - 2] & 0xff) << 8;
            return (buffer[count - 1] & 0xff) | (buffer[count - 2] & 0xff) << 8
                    | (buffer[count - 3] & 0xff) << 16 | (buffer[count - 4] & 0xff) << 24;
        }


        /// <summary>
        /// Read the last two bytes fromt he packet and convert to integer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <returns>Int value of CRC</returns>
        private int ReadCRCFromPacket(byte[] buffer, int count)
        {
            var readCrc = (buffer[count - 1] & 0xff)
                    | (buffer[count - 2] & 0xff) << 8;
            return readCrc;
        }


        /// <summary>
        /// Read the packet from the sensor, Packet read will read the sync frame and make sure 
        /// Packet can be read, later it will read the meta-data and finally the payload.
        /// </summary>
        /// <param name="readFromSd"></param>
        /// <param name="reader"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        private bool ReadPacket(bool readFromSd, BinaryReaderHelper reader, out byte[] packet)
        {
            const int crcByteSize = 2;
            byte[] syncFrame;
            var inSync = ReadSyncFrame(reader, out syncFrame);
            packet = null;
            var readSucessFully = false;
            if (!inSync) return false;
            byte[] receiveBuffer;
            var readPointer = ReadMeta(reader, out receiveBuffer);
            var length = CalculateLength(receiveBuffer, readPointer, readFromSd);
            // Total lenght is current read bytes + payLoadLength of payload read from Meta
            var packetLength = readPointer + length;
            if (packetLength > 600)
                return false;
            // Read The payload + CRC
            // Last 2 bytes read are CRC
            while (readPointer < packetLength)
            {
                receiveBuffer[readPointer++] = reader();
            }
            // Create a packet without CRC
            // Packet inclued Command + NodeID + local Command + local Subcommand 
            // + Payload
            packet = new byte[readPointer - crcByteSize];
            Array.Copy(receiveBuffer, 0, packet, 0, readPointer - crcByteSize);

            //Crc that we get from the packet
            var rcrc = ReadCRCFromPacket(receiveBuffer, readPointer);
            //Computed CRC using meta + payload and no sync frame
            var computedCrc = Crc.Calc(packet, packet.Length);
            if (rcrc == computedCrc)
                readSucessFully = true;
            else
                packet = null;
            return readSucessFully;
        }


        /// <summary>
        /// Writes an AT command specific to sensor type read the 
        /// associative SD card data.
        /// </summary>
        /// <param name="sensorType"></param>
        public async Task<List<Packet>> ReadSdCardData(byte[] sensorType)
        {
            _sensorWriter.WriteFramedPacket(sensorType);
            var packets = await ProcessSdCardPacket();
            return new List<Packet>(packets ?? new List<Packet>());
        }

        /// <summary>
        /// Read the Payload information i.e. File inforamtion about 
        /// specific sensor within the Application.
        /// </summary>
        private async Task<IEnumerable<Packet>> ProcessSdCardPacket()
        {
            byte[] syncFrame;
            byte[] receiveBuffer;
            BinaryReaderHelper myHelper = () => ReadByte(_port);
            var inSync = ReadSyncFrame(myHelper, out syncFrame);
            if (!inSync) return null;
            var readPointer = ReadMeta(myHelper, out receiveBuffer);
            var payloadLength = CalculateLength(receiveBuffer, readPointer, true);
            if (payloadLength <= 4) return null;
            var payloadSavedToFile = await Task.Run(() => ReadPayloadToFile(payloadLength, myHelper));
            var packets = await Task.Run(() => StartProcessingData(payloadLength, payloadSavedToFile));
            return packets;
        }

        /// <summary>
        /// Reads the payload from the sensor and writes it to a temp file
        /// </summary>
        /// <param name="payloadLength"></param>
        /// <param name="serialPortReadhelper"></param>
        /// <returns></returns>
        private bool ReadPayloadToFile(int payloadLength, BinaryReaderHelper serialPortReadhelper)
        {
            _totalPackets = payloadLength;
            const string tempFileName = "temp";
            using (var tempFileWriter = new BinaryWriter(new FileStream(tempFileName, FileMode.Create)))
            {
                var numBytes = 0;
                var endOfStreamCounter = 0;
                while (payloadLength > 0)
                {
                    try
                    {
//                        if (endOfStreamCounter == 10)
//                        {
//                            tempFileWriter.Write(-1 & 0xFF);
//                            numBytes += 1;
//                            _readPackets = numBytes;
//                            payloadLength--;
//                        }
                        var tempData = serialPortReadhelper();
                        if (tempData == unchecked((byte) -1))
                        {
                            endOfStreamCounter++;
                        }
                        else
                        {
                            endOfStreamCounter = 0;
                        }
                        tempFileWriter.Write(tempData);
                        numBytes += 1;
                        _readPackets = numBytes;
                        payloadLength--;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine(@"exited testreading's while");
                        return false;
                    }
                    catch (TimeoutException ex)
                    {
                        tempFileWriter.Write(-1 & 0xFF);
                        numBytes += 1;
                        _readPackets = numBytes;
                        payloadLength--;
                    }
                }
                Console.WriteLine(@"exited testreading's while");
                Console.WriteLine(@"Wrote " + numBytes + @" Bytes");
            }
            return true;
        }

        /// <summary>
        /// Reads the data written on the temp file, 
        /// processes it and creates packets out of it.
        /// </summary>
        /// <param name="payLoadLength"></param>
        /// <param name="payloadSavedToFile"></param>
        private List<Packet> StartProcessingData(int payLoadLength, bool payloadSavedToFile)
        {
            // Collect all packets of sensor in this list
            if (!payloadSavedToFile) return null;
            const string tempFileToRead = "temp";
            var _packets = new List<Packet>();
            using (var tempFileReader = new BinaryReader(new FileStream(tempFileToRead, FileMode.Open)))
            {
                var timeOut = 0;
                while (payLoadLength > 4)
                {
                    var rawPacket = ReadFramedPacket(false, tempFileReader);
                    if (rawPacket != null)
                    {
                        //Add plus 5 to normal packet, 5 is for adding Frame packet(3) and CRC(2)
                        timeOut = 0;
                        payLoadLength = payLoadLength - (rawPacket.Length + 5);
                        var pack = new Packet(rawPacket);
                        if (pack.IsPartialPacket())
                            HandlePartialPackets(pack, ref _packets);
                        else
                        {
                            _packets.Add(pack);
                        }
                    }
                    else
                        timeOut++;
                    if (timeOut > SensorConstants.MTU)
                        break;
                }
            }
            File.Delete(tempFileToRead);
            return _packets;
        }

        /// <summary>
        /// Assemble partial packets into a single packet
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="packets"></param>
        private void HandlePartialPackets(Packet packet, ref List<Packet> packets)
        {
            if (!packet.IsPartialPacket()) return;
            var key = packet.uniqueId();
            //System.out.println("Key:" + key);
            if (_partialData.ContainsKey(key))
            {
                //List<Packet> partialPacket = partialData.get(key);
                List<Packet> partialPacket;
                _partialData.TryGetValue(key, out partialPacket);

                if (partialPacket != null && partialPacket[0].isVibration())
                {
                    if (partialPacket.Count < 15)
                    {
                        partialPacket.Add(packet);
                        _partialData.TryAdd(key, partialPacket);
                    }
                    else
                    {
                        partialPacket.Add(packet);
                        var pack = combinePacket(partialPacket); // replaces above line, publish only sets loading bar
                        List<Packet> removed;
                        _partialData.TryRemove(key, out removed);
                        packets.Add(pack);
                    }
                }

                else if (partialPacket != null && partialPacket[0].isShock())
                {
                    if (partialPacket.Count < 7)
                    {
                        partialPacket.Add(packet);
                        _partialData.TryAdd(key, partialPacket);
                    }
                    else
                    {
                        partialPacket.Add(packet);
                        var pack = combinePacket(partialPacket); // replaces above line, publish only sets loading bar
                        List<Packet> removed;
                        _partialData.TryRemove(key, out removed);
                        packets.Add(pack);
                    }
                }
            }
            else
            {
                var partialPacket = new List<Packet> { packet };
                _partialData.TryAdd(key, partialPacket);
            }
        }

        /// <summary>
        /// Heper method to combine all packets into one Packet
        /// </summary>
        /// <param name="partialPacket"></param>
        /// <returns></returns>
        private Packet combinePacket(IList<Packet> partialPacket)
        {
            var count = 0;
            var pack = new Packet
            {
                NodeId = partialPacket[0].NodeId,
                Service = partialPacket[0].Service,
                ServiceId = partialPacket[0].ServiceId,
                ProtocolType = partialPacket[0].ProtocolType
            };
            pack.setDate(partialPacket[0].getDate());
            pack.addDataToPacket(partialPacket[0].GetPayload()[0]);
            while (partialPacket.Count > 0) 
            {
                Packet temp = null;
                foreach (var packet in partialPacket)
                {
                    temp = packet;
                    if (temp.GetPacketNumber() == count)
                        break;
                }
                if (temp != null)
                {
                    var byteValues = temp.GetPayload();
                    byteValues.RemoveAt(0);
                    foreach (var value in byteValues)
                    {
                        pack.addDataToPacket(value);
                    }
                }
                count++;
                partialPacket.Remove(temp);

            }
            return pack;
        }



        /* Simple Helper functions to get sensor info */


        /// <summary>
        /// Gets the cinfiguration file for the sensor
        /// </summary>
        /// <returns></returns>
        public Packet GetConfiguration()
        {
            _sensorWriter.WriteFramedPacket(SensorConstants.GET_ALL_CONFIG);
            var packet = GetSerialPacket(false);
            return packet;
        }

        /// <summary>
        /// Gets the battery level of the sensor connected
        /// </summary>
        /// <returns></returns>
        public int GetBatteryLevel()
        {
            if (!_port.IsOpen) return -1;
            _sensorWriter.WriteFramedPacket(SensorConstants.GET_BATTERY_LEVEL);
            var packet = GetSerialPacket(true);
            return packet?.GetBatteryLevel() ?? -1;
        }

        /// <summary>
        /// Gets the note written for the current sensor connected
        /// </summary>
        /// <returns></returns>
        public string GetNote()
        {
            _sensorWriter.WriteFramedPacket(SensorConstants.GET_NOTE);
            var packet = GetSerialPacket(true);
            if (packet == null) return string.Empty;
            var byteArray = packet.GetData();
            var charArray = byteArray.Select(Convert.ToChar).ToArray();
            return new string(charArray);
        }

        /// <summary>
        /// Gets the sensor Id of the connected sensor
        /// </summary>
        /// <returns></returns>
        public string GetBoardId()
        {
            _sensorWriter.WriteFramedPacket(SensorConstants.GET_BOARD_ID);
            var packet = GetSerialPacket(false);
            return packet?.SensorId() ?? string.Empty; 
        }

        /// <summary>
        /// Reads the packet
        /// </summary>
        /// <param name="isSdCardData"></param>
        /// <returns></returns>
        private Packet GetSerialPacket(bool isSdCardData)
        {
            var packet = ReadFramedPacket(isSdCardData);
            var packetHelper = packet != null ? new Packet(packet) : null;
            return packetHelper;
        }
        /// <summary>
        /// Clears the SD card data from the sensor.
        /// </summary>
        private void ClearSdCardData() {
            _sensorWriter.WriteFramedPacket(SensorConstants.FORMAT_SD_CARD);
            var packet = GetSerialPacket(false);
        }
        /// <summary>
        /// Clear the Flash Storage of the Sensor module
        /// </summary>
        private void ClearFlashData()
        {
            _sensorWriter.WriteFramedPacket(SensorConstants.FORMAT_FLASH);
            var packet = GetSerialPacket(false);
        }

        /// <summary>
        /// Clear the moemory of the sensor module.
        /// </summary>
        private void ClearData()
        {
            ClearSdCardData();
            if(_isFashAvailable)
                ClearFlashData();
            
        }
        /// <summary>
        /// Check if the sensor module has flah storage
        /// </summary>
        /// <returns></returns>
        public bool IsFlashDataAvailable()
        {
            var packet = GetConfiguration();
            _isFashAvailable = (packet.getData(SensorConstants.DATA_IN_FLASH_INDEX) & 0xff) != 0;
            return _isFashAvailable;
        }

        /// <summary>
        /// Delete the sceduled start and end time programmed within the sensor module.
        /// </summary>
        private void DeleteSchedule()
        {
            _sensorWriter.WriteFramedPacket(SensorConstants.DELETE_SCHEDULE);
            var packet = GetSerialPacket(false);
        }

        public bool Configure(byte[] configPacket, byte[] note)
        {
            Console.WriteLine("Clearing sensor data");
            ClearData();
            Console.WriteLine("Clearing sensor data complete");
            _sensorWriter.WriteStreamedPacket(SensorConstants.CONFIG_SERVICE, configPacket);
            _sensorWriter.WriteFramedPacket(SensorConstants.SAVE_ALL_CONFIG);
            var packet = GetSerialPacket(false);
            _sensorWriter.WriteFramedPacket(SensorConstants.SAVE_COMMENT, note);
            packet = GetSerialPacket(false);
            DeleteSchedule();
            SendTimeSyncPacket();
            Console.WriteLine("Time Sync Done");
            _sensorWriter.WriteFramedPacket(SensorConstants.P_UNKNOWN);
            packet = GetSerialPacket(false);
            return packet.getDate().Date.Equals(DateTime.Now.Date);
        }

        private void SendTimeSyncPacket(){
           
            var currentTime = CurrentTimeMillis() / 1000;
            var timePack = BigInteger.Parse($"{currentTime:X}", NumberStyles.HexNumber).ToByteArray().Reverse().ToArray();           
            _sensorWriter.WriteFramedPacket(SensorConstants.P_TIME_SYNC, timePack);
             
        }

    }
}
