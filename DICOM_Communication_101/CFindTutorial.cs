using EvilDICOM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_Communication_101
{
    public class CFindTutorial
    {    
        /// <summary>
         /// This tutorial is outlined in chapter 4 of Scripting in RT for Physicists (C-FIND)
         /// </summary>
        public static void Run()
        {
            //Store the details of the daemon (Ae Title, IP, port)
            var daemon = new Entity("PHYSX_DICOM", "10.22.86.64", 51402);
            //Store the details of the client (Ae Title, port) -> IP address is determined by CreateLocal() method
            var local = Entity.CreateLocal("DICOMEC1", 9999);
            //Set up a client (DICOM SCU = Service Class User)
            var client = new DICOMSCU(local);
            //Build a finder class to help with C-FIND operations
            var finder = client.GetCFinder(daemon);

            var studies = finder.FindStudies("DA00001");
            var series = finder.FindSeries(studies);
            var images = finder.FindImages(series);

            //Write results to console
            Console.WriteLine($"DICOM C-Find from {local.AeTitle} => " +
                    $"{daemon.AeTitle} @{daemon.IpAddress}:{daemon.Port}:");
            Console.WriteLine($"{studies.Count()} Studies Found");
            Console.WriteLine($"{series.Count()} Series Found");
            Console.WriteLine($"{images.Count()} Images Found");
            Console.Read(); //Stop here
        }
    }
}
