using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UMS
{
    public struct Result
    {
        public Result(bool succeeded)
        {
            _succeeded = succeeded;
            _messages = new List<Message>();
        }
        public Result(object message, MessageType type)
        {
            _succeeded = type != MessageType.Exception && type != MessageType.Error;
            _messages = new List<Message>()
            {
                new Message()
                {
                    message = message,
                    type = type,
                },
            };

#if DEBUG
            Output();
#endif
        }
        public Result(object message, Data data, MessageType type)
        {
            _succeeded = type != MessageType.Exception && type != MessageType.Error;
            _messages = new List<Message>()
            {
                new Message()
                {
                    message = message + System.Environment.NewLine + data,
                    type = type,
                },
            };

#if DEBUG
            Output();
#endif
        }

        #region Static Declarations
        public static readonly Result Success = new Result(true);
        public static readonly Result Fail = new Result(false);
        public static readonly Result Null = new Result();
        #endregion

        #region Static Functions
        public static Result Info(string message)
        {
            return new Result(message, MessageType.Info);
        }
        public static Result Info(string message, Data data)
        {
            return new Result(message, data, MessageType.Info);
        }
        public static Result Warn(string message)
        {
            return new Result(message, MessageType.Warning);
        }
        public static Result Warn(string message, Data data)
        {
            return new Result(message, data, MessageType.Warning);
        }
        public static Result Error(string message)
        {
            return new Result(message, MessageType.Error);
        }
        public static Result Error(string message, Data data)
        {
            return new Result(message, data, MessageType.Error);
        }
        public static Result Exception(System.Exception exception)
        {
            return new Result(exception, MessageType.Exception);
        }
        public static Result Exception(System.Exception exception, Data data)
        {
            return new Result(exception, data, MessageType.Exception);
        }
        #endregion

        public bool Succeeded { get { return _succeeded; } }
        public bool Failed { get { return !Succeeded; } }

        private bool _succeeded;
        private List<Message> _messages;

        public string FormattedMessage { get { return string.Join(System.Environment.NewLine, _messages.Select(x => x.message.ToString()).ToArray()); } }
        
        public void AddInfo(object message)
        {
            _messages.Add(new Message(message, MessageType.Info));
        }
        public void AddWarning(object message)
        {
            _messages.Add(new Message(message, MessageType.Warning));
        }
        public void AddError(object message)
        {
            _messages.Add(new Message(message, MessageType.Error));
        }
        public void AddException(System.Exception exception)
        {
            _messages.Add(new Message(exception, MessageType.Exception));
        }
        public void Output()
        {
            foreach (Message message in _messages)
            {
                if(message.type == MessageType.Exception || message.type == MessageType.Error || message.type == MessageType.Warning)
                    message.Output();
            }
        }

        /// <summary>
        /// Asserts that no errors occured
        /// Throws an exception if there did
        /// </summary>
        public void Assert()
        {
            if (!Succeeded)
                throw new System.Exception(FormattedMessage);
        }
        /// <summary>
        /// Asserts that no errors or warning occured
        /// Throws an exception if there did
        /// </summary>
        public void AssertWithoutWarnings()
        {
            Assert();

            if (_messages.Any(x => x.type == MessageType.Warning))
                throw new System.Exception(FormattedMessage);
        }

        public override string ToString()
        {
            return FormattedMessage;
        }
        public static Result operator +(Result a, Result b)
        {
            return new Result()
            {
                _succeeded = !a._succeeded || !b._succeeded ? false : true,
                _messages = new List<Message>(a._messages.Union(b._messages)),
            };
        }

        #region Subdefintions
        private struct Message
        {
            public Message(object message, MessageType type = MessageType.Info)
            {
                this.message = message;
                this.type = type;
            }

            public object message;
            public MessageType type;

            public void Output()
            {
                switch (type)
                {
                    case MessageType.Info:
                        Debug.Log(message);
                        break;
                    case MessageType.Warning:
                        Debug.LogWarning(message);
                        break;
                    case MessageType.Error:
                        Debug.LogError(message);
                        break;
                    case MessageType.Exception:
                        Debug.LogException(message as System.Exception);
                        break;
                    default:
                        throw new System.Exception("Unexpected type " + type);
                }
            }
            public override string ToString()
            {
                return message.ToString();
            }
        }
        public enum MessageType
        {
            None = 0,

            Info = 1,
            Warning = 2,
            Error = 3,
            Exception = 4,
        }
        #endregion
    }
}
