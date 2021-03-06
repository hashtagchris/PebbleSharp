﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PebbleSharp.Core.Bundles;
using PebbleSharp.Core.Tests.Responses;
using System;
using System.Threading.Tasks;

namespace PebbleSharp.Core.Tests
{
    [TestClass]
    public class PebbleTests
    {
        private const string TEST_PEBBLE_ID = "TESTID";

        [TestMethod]
        [TestCategory( "Core" )]
        public void ConstructorTest()
        {
            var bluetoothConnection = new Mock<IBluetoothConnection>( MockBehavior.Strict );

            var pebble = new TestPebble( bluetoothConnection.Object, TEST_PEBBLE_ID );

            Assert.AreEqual( TEST_PEBBLE_ID, pebble.PebbleID );
        }

        #region InstallFirmwareAsync Tests

        [TestMethod, ExpectedException( typeof( ArgumentNullException ) )]
        [TestCategory( "Core" )]
        public async Task InstallFirmwareAsyncRequiresBundle()
        {
            var bluetoothConnection = new Mock<IBluetoothConnection>();
            var pebble = new TestPebble( bluetoothConnection.Object, TEST_PEBBLE_ID );

            await pebble.InstallClient.InstallFirmwareAsync( null );
        }

        [TestMethod]
        [TestCategory( "Core" )]
        public async Task InstallFirmwareAsyncTest()
        {
            var bundle = new Mock<FirmwareBundle>();
            var firmwareBytes = new byte[16];
            var resourceBytes = new byte[4];
            bundle.SetupGet( x => x.HasResources ).Returns( true );
            bundle.SetupGet( x => x.Resources ).Returns( resourceBytes );
            bundle.SetupGet( x => x.Firmware ).Returns( firmwareBytes );

            var bluetoothConnection = new Mock<IBluetoothConnection>( MockBehavior.Strict );

            //Resource and firmware headers
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 11 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.PutBytes ) )
                .Verifiable();
            //Resource data
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 13 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.PutBytes ) )
                .Verifiable();
            //Firmware data
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 25 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.PutBytes ) )
                .Verifiable();
            //Resource and Firmware CRC
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 9 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.PutBytes ) )
                .Verifiable();
            //Put bytes complete message
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 5 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.PutBytes ) )
                .Verifiable();
            //Send and complete firmware system messages
            bluetoothConnection.Setup( x => x.Write( It.Is<byte[]>( b => b.Length == 6 ) ) )
                .Raises( x => x.DataReceived += null, bluetoothConnection, ResponseGenerator.GetBytesReceivedResponse( Endpoint.SystemMessage ) )
                .Verifiable();

            var pebble = new TestPebble( bluetoothConnection.Object, TEST_PEBBLE_ID );

            bool success = await pebble.InstallClient.InstallFirmwareAsync( bundle.Object );
            Assert.IsTrue( success );
            bluetoothConnection.Verify();
        }

        #endregion InstallFirmwareAsync Tests

        private class TestPebble : Pebble
        {
            public TestPebble( IBluetoothConnection connection, string pebbleId )
                : base( connection, pebbleId )
            {
            }
        }
    }
}
