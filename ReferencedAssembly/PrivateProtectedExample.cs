using System;

namespace ReferencedAssembly
{
    public class ProtectedPrivateBaseClass
    {
        
        protected int protectedValue; //Any derived class can access this
        protected internal int protectedInternalValue; //If a class is derived OR in the same assembly it can access this

        //The protected private access modifer was added in C# 7.2
        protected private int protectedPrivateValue; //If a class is derived AND in the same assembly it can access this

        public ProtectedPrivateBaseClass()
        {
            protectedPrivateValue = 1;
            protectedInternalValue = 1;
            protectedValue = 1;
        }
    }

    public class ChildClassInSameAssembly: ProtectedPrivateBaseClass
    {
        public int Calculate()
        {
            return protectedPrivateValue + protectedInternalValue + protectedValue;
        }
    }

    public class NonChildClassInSameAssembly
    {
        private ProtectedPrivateBaseClass composedObject = new ProtectedPrivateBaseClass();

        public int Calculate()
        {
            return composedObject.protectedInternalValue;
        }
    }
}
