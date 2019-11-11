using System;
using System.IO.Ports;
using System.Threading;
using PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Util;
using PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Writer;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Globalization;
using System.Management;
using System.Linq;
using PakTrack.DAL.Context;
using PakTrack.DAL.Interfaces;
using PakTrack.Models;
using System.Threading.Tasks;

namespace PakTrack.SensorsConfigurator.SensorDataReader.Rtpmt.Sensor.Reader
{
    //this class is actually the BlackBoxReader class
    public class BlackBoxReader
    {
        private readonly SerialPort Port;
        private SensorWriter sensorWriter;
        private readonly int NODE_ID = 0;
        private readonly ConcurrentDictionary<string, List<Packet>> partialData;

        private string truckId;
        private string packageId;
        public string sensorId;
        private delegate byte BinaryReaderHelper();

        private IPakTrackDbContext dbContext;


        public BlackBoxReader(SerialPort p)
        {
            Port = p;
            sensorWriter = new SensorWriter(Port);
            partialData = new ConcurrentDictionary<string, List<Packet>>();
            dbContext = new PakTrackDbContext(GetConnectionString());
        }

        public Packet GetConfiguration()
        {
            sensorWriter.WriteFramedPacket(Constants.GET_ALL_CONFIG);
            var packet = GetSerialPacket(false);
            return packet;
        }

        private int GetBatteryLevel()
        {
            sensorWriter.WriteFramedPacket(Constants.GET_BATTERY_LEVEL);
            var packet = GetSerialPacket(true);
            return packet.GetBatteryLevel();
        }

        private string GetNote()
        {
            sensorWriter.WriteFramedPacket(Constants.GET_NOTE);
            var packet = GetSerialPacket(true);
            var byteArray = packet.GetData();
            var charArray = new char[byteArray.Length];
            for (var i = 0; i < charArray.Length; i++)
            {
                charArray[i] = Convert.ToChar(byteArray[i]);
            }
            var note = new string(charArray);
            var noteArray = note.Split('-');
            if (noteArray.Length <= 2) return new string(charArray);
            this.truckId = noteArray[0];
            this.packageId = noteArray[1];
            return new string(charArray);
        }

        public string GetBoardId()
        {
            sensorWriter.WriteFramedPacket(Constants.GET_BOARD_ID);
            var packet = GetSerialPacket(false);
            sensorId = packet.SensorId();
            return sensorId;
        }

        public Packet GetSerialPacket(bool isSdCardData)
        {
            var packet = ReadFramedPacket(isSdCardData);
            var packetHelper = new Packet(packet);
            return packetHelper;
        }


        /// <summary>
        /// Read the sync frame from files and ports
        /// </summary>
        /// <param name="reader">Source from where the sync frame should be read</param>
        /// <param name="syncFrame"></param>
        /// <returns>Boolen, true if read sucessfully else false</returns>
        private bool ReadSyncFrame(BinaryReaderHelper reader,  out byte[] syncFrame)
        {
            var syncFrameByte = 0;
            var notInSyncCount = 0;
            var syncFrameIndex = 0;
            syncFrame = new byte[Constants.FRAME_SYNC.Length]; 
            //Loop till we find first byte of the SyncFrame
            while (syncFrameByte != 170) //170 = OxAA
            {
                try
                {
                    syncFrameByte = reader() & 0xff;
                    notInSyncCount++;
                    if (notInSyncCount > Constants.MTU)
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

            while (syncFrameIndex < Constants.FRAME_SYNC.Length && notInSyncCount < Constants.MTU)
            {
                syncFrameByte = reader();
                syncFrame[syncFrameIndex++] = (byte)(syncFrameByte & 0xff);
                notInSyncCount++;
            }

            return Compare(syncFrame, Constants.FRAME_SYNC) && notInSyncCount < Constants.MTU;
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
            int readPointer = 0;
            buffer = new byte[Constants.MTU];

            // Read request/response Command 1 Byte Always D1
            // This commaond diffrentiates the request packet from 
            // resposne packet 
            byte command = reader();
            buffer[readPointer++] = command;
            //Read node Id 2 bytes
            buffer[readPointer++] = reader();
            buffer[readPointer++] = reader();
            //Read Length 2 Bytes
            buffer[readPointer++] = reader();
            buffer[readPointer++] = reader();
            
            return readPointer;
        }

        private int CalculateLength(byte[] buffer, int count, bool readFromSd)
        {
            if (!readFromSd)
            {
                return (buffer[count - 1] & 0xff) | (buffer[count - 2] & 0xff) << 8;
            }
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
            int readCrc = (buffer[count - 1] & 0xff)
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
            // Total lenght is current read bytes + length of payload read from Meta
            var packetLength = readPointer + length;
            // Read The payload + CRC
            // Last 2 bytes read are CRC
            while(readPointer < packetLength)
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
        /// Read a packet frame from the sensor or filestream if provided
        /// </summary>
        /// <param name="readSdCard"></param>
        /// <param name="input">optional parameter filestream</param>
        /// <returns>A packet frame byte array representations</returns>
        public byte[] ReadFramedPacket(bool readSdCard, BinaryReader input = null)
        {
            BinaryReaderHelper myHelper = () => (byte)Port.ReadByte();
            myHelper = input != null ? input.ReadByte : myHelper;
            byte[] receiveBuffer;
            ReadPacket(readSdCard, myHelper, out receiveBuffer);
            return receiveBuffer;
        }


        public static bool Compare(byte[] first, int[] second) // from Utils.Compare
        {
            var isEqual = true;

            if (first.Length > 0 && second.Length > 0 && first.Length == second.Length)
            {

                if (second.Where((t, i) => (first[i] & 0xff) != t).Any())
                {
                    isEqual = false;
                }
            }
            else
            {
                isEqual = false;
            }
            return isEqual;
        }

      

        private void ReadSDCardData()
        { //throws IOException, InterruptedException, TimeoutException {

            //Package pack = PackageList.getPackage(NODE_ID);UNCOMMENT AND IMPLEMENT THIS
            //if (pack.isFlashDataAvailable())
            //{
            //    //System.err.print("Reading flash data");
            //    Console.WriteLine("Reading flash data");
            //    ReadSDCardData(Constants.GET_FLASH);
            //}

            //System.err.println("Reading Temperature");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_TEMPERATURE);

            //System.err.println("Reading Humididty");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_HUMIDITY);

            //System.err.println("Reading Vibration");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_VIBRATION);

            //System.err.println("Reading Vibration");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_SHOCK);

            //System.err.println("Reading Light");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_LIGHT);

            //System.err.println("Reading Pressure");
            Console.WriteLine(@"exited testreading's while");
            ReadSDCardData(Constants.GET_PRESSURE);

            //sensorWriter.writeFramedPacket(Constants.FORMAT_FLASH);
            //Packet packet = getSerialPacket(false);
        }

        private void ReadSDCardData(byte[] sensorType)
        {//throws IOException, InterruptedException, TimeoutException{
            sensorWriter.WriteFramedPacket(sensorType);
            ProcessSDCardPacket();
        }


        public async Task ProcessSDCardPacket()
        {
            byte[] syncFrame;
            byte[] receiveBuffer;
            BinaryReaderHelper myHelper = () => (byte)Port.ReadByte();
            var inSync = ReadSyncFrame(myHelper, out syncFrame);
            if (!inSync) return;
            var readPointer = ReadMeta(myHelper, out receiveBuffer);
            var payloadLength = CalculateLength(receiveBuffer, readPointer, true);
            if (payloadLength <= 4) return;
            var payloadSavedToFile = await Task.FromResult (ReadPayloadToFile(payloadLength, myHelper));
            StartProcessingData(payloadLength, payloadSavedToFile);
        }

        private bool ReadPayloadToFile(int payloadLength, BinaryReaderHelper serialPortReadhelper)
        {
            const string tempFileName = "temp";
            using (var tempFileWriter = new BinaryWriter(new FileStream(tempFileName, FileMode.Create)))
            {
                try
                {
                    int numBytes = 0;

                    while (payloadLength > 0)
                    {
                        var tempData = serialPortReadhelper();
                        tempFileWriter.Write(tempData);
                        numBytes += 1;
                        payloadLength--;
                    }
                    Console.WriteLine(@"exited testreading's while");
                    Console.WriteLine("Wrote " + numBytes + " Bytes");
                }
                catch (InvalidOperationException)
                {
                    Console.WriteLine(@"exited testreading's while");
                    return false;
                }
                catch (TimeoutException)
                {
                    while (payloadLength > 0)
                    {
                        // Fill rest of the file with -1 as that is the interupt for End of File
                        tempFileWriter.Write(-1 & 0xFF);
                        payloadLength--;
                    }
                    return true;
                }
            }
            Console.WriteLine(@"exited testreading's while");
            return true;

        }

        private void StartProcessingData(int payLoadLength, bool payloadSavedToFile)
        {
            if (!payloadSavedToFile) return;
            const string tempFileToRead = "temp";
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
                        if (pack.isPartialPacket())
                        {
                            handlePartialPackets(pack);
                        }
                        else
                        {
                            //TODO: We got the packet now need to save it in db
                            pack.getSensorMessage();
                            //dbContext.Temperatures.Insert(new Temperature()
                            //{
                            //    PackageId = this.packageId,
                            //    TruckId = this.truckId,
                            //    SensorId = sensorId,
                            //    Unit = "F",
                            //    Value = pack.getTemperature(),
                            //    Timestamp = pack.getDate().Ticks,
                            //    IsAboveThreshold = false

                            //});
                        }
                    }
                    else
                    {
                        timeOut++;
                    }
                    if (timeOut > Constants.MTU)
                        break;
                }
            }
            File.Delete(tempFileToRead);
        }



        private void handlePartialPackets(Packet packet)
        {
            if (packet.isPartialPacket())
            {
                string key = packet.uniqueId();
                //System.out.println("Key:" + key);
                if (partialData.ContainsKey(key))
                {
                    //List<Packet> partialPacket = partialData.get(key);
                    List<Packet> partialPacket;
                    partialData.TryGetValue(key, out partialPacket);

                    if (partialPacket != null && partialPacket[0].isVibration())
                    {
                        if (partialPacket.Count < 15)
                        {
                            partialPacket.Add(packet);
                            partialData.TryAdd(key, partialPacket);
                        }
                        else
                        {
                            partialPacket.Add(packet);
                            //publishNewPacket(combinePacket(partialPacket));
                            Packet pack = combinePacket(partialPacket); // replaces above line, publish only sets loading bar
                            List<Packet> removed;
                            partialData.TryRemove(key, out removed);
                            pack.getSensorMessage();
                            //dbContext.Temperatures.Insert(new Temperature()
                            //{
                            //    PackageId = this.packageId,
                            //    TruckId = this.truckId,
                            //    SensorId = sensorId,
                            //    Unit = "F",
                            //    Value = pack.getTemperature(),
                            //    Timestamp = pack.getDate().Ticks,
                            //    IsAboveThreshold = false

                            //});
                        }
                    }

                    else if (partialPacket != null && partialPacket[0].isShock())
                    {
                        if (partialPacket.Count < 7)
                        {
                            partialPacket.Add(packet);
                            partialData.TryAdd(key, partialPacket);
                        }
                        else
                        {
                            partialPacket.Add(packet);
                            //publishNewPacket(combinePacket(partialPacket));
                            var pack = combinePacket(partialPacket); // replaces above line, publish only sets loading bar
                            List<Packet> removed;
                            partialData.TryRemove(key, out removed);
                            pack.getSensorMessage();
 
                        }
                    }

                    /*if (partialpacket.isCompletePacket(packet)) {
                        partialpacket.combinePacket(packet);
                        publishNewPacket(partialData.remove(key));
                    } else {
                        System.out.println("mismatch packet");
                        Dump.dump(packet);
                    }*/

                }
                else
                {
                    var partialPacket = new List<Packet> {packet};
                    partialData.TryAdd(key, partialPacket);
                }
            }
        }

        private string GetConnectionString()
        {
            var path = Directory.GetCurrentDirectory();
            var dbLocation = ConfigurationManager.AppSettings["DbLocation"];
            var dbPath = Path.GetFullPath(Path.Combine(path, dbLocation));
            return dbPath;
        }

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
            pack.addDataToPacket(partialPacket[0].getPayload()[0]);
            while (partialPacket.Count > 0) //(!partialPacket.isEmpty())
            {
                Packet temp = null;
                foreach (Packet packet in partialPacket)
                {
                    temp = packet;
                    if (temp.getPacketNumber() == count)
                        break;
                }
                List<byte> byteValues = temp.getPayload();
                byteValues.RemoveAt(0);
                foreach (var value in byteValues)
                {
                    pack.addDataToPacket(value);
                }
                count++;
                partialPacket.Remove(temp);

            }
            //System.out.println("total length of payload " + pack.getPayload().size());
            //Console.WriteLine("total length of payload " + pack.getPayload().Count);
            return pack;
        }

        public static string GetCorrectPort()
        {
            var ComName = Utilities.SensorConstants.SensorDiscoveryName;
            //COM6 - TI MSP430 USB (COM6)
            using (var searcher = new ManagementObjectSearcher
               ("SELECT * FROM WIN32_SerialPort Where Caption Like '%"+ ComName +"%'"))
            {
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                return ports.First()["DeviceID"].ToString();
            }
        }

        static void Main(string[] args) // not really a part of this
        {
            //string correctPort = GetCorrectPort();
            

            //SerialPort NewPort = new SerialPort(correctPort);
            //Console.WriteLine("Name is :" + NewPort.PortName);
            //NewPort.BaudRate = 230400;
            //NewPort.DataBits = 8;
            //NewPort.StopBits = StopBits.One;
            //NewPort.Parity = Parity.None;
            //NewPort.ReadTimeout = 100;
            
            //NewPort.Open();
            //var BReader = new BlackBoxReader(NewPort);
            
            //Console.WriteLine("Obtained sensor id: " + BReader.GetBoardId());
            //Console.WriteLine("Obtained battery level: " + BReader.GetBatteryLevel());
            //Console.WriteLine("Obtained note: " + BReader.GetNote());
            //BReader.ReadSDCardData(Constants.GET_LIGHT);

            //NewPort.Close();
            //Console.ReadKey();

            var timestamp = 1486348200;
            var epocTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var dtDateTime = epocTime.AddSeconds(timestamp).ToLocalTime();
            Console.WriteLine(dtDateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            var timestamp2 = dtDateTime.ToUniversalTime().Subtract(epocTime).TotalMilliseconds;
            Console.WriteLine(timestamp2);
            Console.WriteLine(timestamp);
            Console.ReadKey();

        }
    }
}
