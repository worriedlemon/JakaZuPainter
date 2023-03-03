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
        private readonly JsonObject _jsonObject;
        public string RawJson { get; private set; }
        public string CmdName { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMsg { get; private set; }

        public SendingResponse(string jsonString)
        {
            RawJson = jsonString;
            _jsonObject = JsonNode.Parse(jsonString)!.AsObject();
            CmdName = _jsonObject["cmdName"]!.ToString();
            ErrorCode = _jsonObject["errorCode"]!.ToString();
            ErrorMsg = _jsonObject["errorMsg"]!.ToString();
        }
    }

    public class RobotDataResponse
    {
        public RobotDataResponse(string rawJson)
        {
        }

        public JointPositions JointPositions { get; private set; }
    }
}
