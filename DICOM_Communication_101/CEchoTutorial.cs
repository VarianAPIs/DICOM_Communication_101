using EvilDICOM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM_Communication_101
{ 
    public class CEchoTutorial
    {   
        /// <summary>
        /// This tutorial is outlined in chapter 4 of Scripting in RT for Physicists (C-ECHO)
        /// </summary>
        public static void Run()
        {
            //Store the details of the daemon (Ae Title, IP, port)
            var daemon = new Entity("PHYSX_DICOM", "10.22.86.64", 51402);
            //Store the details of the client (Ae Title, port) -> IP address is determined by CreateLocal() method
            var local = Entity.CreateLocal("DICOMEC1", 9999);
            //Set up a client (DICOM SCU = Service Class User)
            var client = new DICOMSCU(local);
            //TRY C-ECHO
            var canPing = client.Ping(daemon);
            //Write results to console
            Console.WriteLine($"DICOM C-Echo from {local.AeTitle} => " +
                $"{daemon.AeTitle} @{daemon.IpAddress}:{daemon.Port} was successfull? {canPing}");

            Console.Read(); //Stop here
        }
    }
}
