using EvilDICOM.Core.Helpers;
using EvilDICOM.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_Communication_101
{
    /// <summary>
    /// This tutorial is outlined in chapter 4 of Scripting in RT for Physicists (C-Move to Self)
    /// </summary>
    public class CMoveTutorial2
    {
        public static void Run()
        {
            //Store the details of the daemon (Ae Title, IP, port)
            var daemon = new Entity("PHYSX_DICOM", "10.22.86.64", 51402);
            //Store the details of the client (Ae Title, port) -> IP address is determined by CreateLocal() method
            var local = Entity.CreateLocal("DICOMEC1", 9999);
            //Set up a client (DICOM SCU = Service Class User)
            var client = new DICOMSCU(local);
            //Set up a receiver to catch the files as they come in
            var receiver = new DICOMSCP(local);
            //Let the daemon know we can take anything it sends
            receiver.SupportedAbstractSyntaxes = AbstractSyntax.ALL_RADIOTHERAPY_STORAGE;
            //Set up storage location
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var storagePath = Path.Combine(desktopPath, "DICOM Storage");
            Directory.CreateDirectory(storagePath);
            //Set the action when a DICOM files comes in
            receiver.DIMSEService.CStoreService.CStorePayloadAction = (dcm, asc) =>
            {
                var path = Path.Combine(storagePath, dcm.GetSelector().SOPInstanceUID.Data + ".dcm");
                Console.WriteLine($"Writing file {path}...");
                dcm.Write(path);
                return true; // Lets daemom know if you successfully wrote to drive
};
            receiver.ListenForIncomingAssociations(true);

            //Build a finder class to help with C-FIND operations
            var finder = client.GetCFinder(daemon);
            var studies = finder.FindStudies("DA00001");
            var series = finder.FindSeries(studies);

            //Filter series by modality, then create list of 
            var plans = series.Where(s => s.Modality == "RTPLAN")
                .SelectMany(ser => finder.FindImages(ser));
            var doses = series.Where(s => s.Modality == "RTDOSE")
                    .SelectMany(ser => finder.FindImages(ser));
            var cts = series.Where(s => s.Modality == "CT")
                    .SelectMany(ser => finder.FindImages(ser));

            var mover = client.GetCMover(daemon);
            ushort msgId = 1;
            foreach (var plan in plans)
            {
                Console.WriteLine($"Sending plan {plan.SOPInstanceUID}...");
                //Make sure Mobius is on the whitelist of the daemon
                var response = mover.SendCMove(plan, local.AeTitle, ref msgId);
                Console.WriteLine($"DICOM C-Move Results : ");
                Console.WriteLine($"Number of Completed Operations : {response.NumberOfCompletedOps}");
                Console.WriteLine($"Number of Failed Operations : {response.NumberOfFailedOps}");
                Console.WriteLine($"Number of Remaining Operations : {response.NumberOfRemainingOps}");
                Console.WriteLine($"Number of Warning Operations : {response.NumberOfWarningOps}");
            }

            Console.Read(); //Stop here
        }
    }
}
