using EvilDICOM.Core;
using EvilDICOM.Network;
using EvilDICOM.Network.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_Communication_101
{
    public class CStoreTutorial
    {
        /// <summary>
        /// This tutorial is outlined in chapter 4 of Scripting in RT for Physicists (C-Store)
        /// </summary>
        public static void Run()
        {
            //Store the details of the daemon (Ae Title, IP, port)
            var daemon = new Entity("PHYSX_DICOM", "10.22.86.64", 51402);
            //Store the details of the client (Ae Title, port) -> IP address is determined by CreateLocal() method
            var local = Entity.CreateLocal("DICOMEC1", 9999);
            //Set up a client (DICOM SCU = Service Class User)
            var client = new DICOMSCU(local);
            var storer = client.GetCStorer(daemon);

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var storagePath = Path.Combine(desktopPath, "DICOM Storage");

            ushort msgId = 1;
            var dcmFiles = Directory.GetFiles(storagePath);
            foreach (var path in dcmFiles)
            {
                //Reads DICOM object into memory
                var dcm = DICOMObject.Read(path);
                var response = storer.SendCStore(dcm, ref msgId);
                //Write results to console
                Console.WriteLine($"DICOM C-Store from {local.AeTitle} => " +
                        $"{daemon.AeTitle} @{daemon.IpAddress}:{daemon.Port}:" +
                        $"{(Status)response.Status}");
            }
            Console.Read(); //Stop here
        }
    }
}
