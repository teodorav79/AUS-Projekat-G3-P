using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;
            byte[] request = new byte[12];

            request[0] = (byte)(p.TransactionId >> 8);
            request[1] = (byte)(p.TransactionId & 0xFF);
            request[2] = 0;
            request[3] = 0;
            request[4] = 0;
            request[5] = 6;
            request[6] = p.UnitId;
            request[7] = p.FunctionCode;
            request[8] = (byte)(p.StartAddress >> 8);
            request[9] = (byte)(p.StartAddress & 0xFF);
            request[10] = (byte)(p.Quantity >> 8);
            request[11] = (byte)(p.Quantity & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();
            ModbusReadCommandParameters p = (ModbusReadCommandParameters)CommandParameters;

            int byteCount = response[8];
            for (int i = 0; i < p.Quantity; i++)
            {
                int byteIndex = 9 + i / 8;
                int bitIndex = i % 8;
                ushort value = ((ushort)((response[byteIndex] >> bitIndex) & 0x01));
                result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_INPUT, (ushort)(p.StartAddress + i)), value);
                 
            }
            return result;

        }
    }
}