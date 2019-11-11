using System;
using System.Linq;
using PakTrack.Utilities;
using Prism.Mvvm;
using Prism.Regions;


namespace PakTrack.Core.Base
{
    /// <summary>
    ///     This base class contain properties and method common to all PakTrack viewmodel
    /// </summary>
    public abstract class PakTrackBindableBase : BindableBase
    {
        private string _message = "PakTrack Application";

        private string _packageId;

        private string _title = "PakTrack Application";

        private string _truckId;

        protected bool IsFlashDataAvailable = false;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string TruckId
        {
            get { return _truckId; }
            set { SetProperty(ref _truckId, value); }
        }

        public string PackageId
        {
            get { return _packageId; }
            set { SetProperty(ref _packageId, value); }
        }

        protected bool IsActiveView(IRegion regionInfo, string viewName)
        {
            var result = false;
            var activeVIew = regionInfo.ActiveViews.FirstOrDefault();
            if (activeVIew != null)
                if (viewName.Equals(activeVIew.GetType().Name))
                    result = true;

            return result;
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            TruckId = navigationContext.Parameters[PakTrackConstant.TruckId].ToString();
            PackageId = navigationContext.Parameters[PakTrackConstant.PackageId].ToString();
            Initialize();
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        //When leaving
        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        /// <summary>
        /// Need to be implemented
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Method invoked when truck and package change. It assigns the truck and package Ids
        /// </summary>
        /// <param name="navigationInformation">Object with realted information to truck and </param>
        protected virtual void OnTruckAndPackageChanged(NavigationInformation navigationInformation)
        {
            TruckId = navigationInformation.TruckId;
            PackageId = navigationInformation.PackageId;
        }
    }
}