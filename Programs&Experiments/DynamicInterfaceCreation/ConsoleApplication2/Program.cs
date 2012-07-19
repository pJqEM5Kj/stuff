using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;

namespace ConsoleApplication2
{
    interface ITestInterface
    {
        string Person { get; }
        void Method();
        int Method2(string s);
        void MMM(ref int i);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Func<IMethodCallMessage, IMessage> imp =
                (IMethodCallMessage mcm) =>
                {
                    Console.WriteLine(mcm.MethodName);

                    return new MethodResponse(null, mcm);
                };

            var dp = new DynamicProxy(imp);
            ITestInterface bar = (ITestInterface)dp.GetTransparentProxy();

            //
            bar.Method();

            int i = 0;
            //bar.MMM(ref i);

            //i = bar.Method2("");
        }
    }

    class DynamicProxy : RealProxy, IRemotingTypeInfo
    {
        private Func<IMethodCallMessage, IMessage> _implementor;


        public DynamicProxy(Func<IMethodCallMessage, IMessage> implementor)
            : base(typeof(MarshalByRefObject))
        {
            _implementor = implementor;
        }

        public bool CanCastTo(Type fromType, object o)
        {
            return true;
        }

        public override IMessage Invoke(IMessage msg)
        {
            return _implementor((IMethodCallMessage)msg);
        }

        public string TypeName
        {
            get
            {
                return typeof(MarshalByRefObject).FullName;
            }
            set { }
        }
    }
}
