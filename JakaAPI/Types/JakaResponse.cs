using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;

namespace JakaAPI.Types
{
    public interface IResponse { };

    public class SendingResponse
    {
        public SendingResponse(string jsonString)
        {
            rawJson = jsonString;

            _jsonObject = JsonNode.Parse(jsonString).AsObject();

            cmdName = _jsonObject["cmdName"].ToString();

            errorCode = _jsonObject["errorCode"].ToString();

            errorMsg = _jsonObject["errorMsg"].ToString();
        }

        private readonly JsonObject _jsonObject;

        public string rawJson { get; private set; }

        public string cmdName { get; private set; }

        public string errorCode { get; private set; }

        public string errorMsg { get; private set; }
    }

    public class RobotDataResponse
    {
        public RobotDataResponse(string rawJson)
        {
        }

        public JointPositions jointPositions { get; private set; }
    }
}
