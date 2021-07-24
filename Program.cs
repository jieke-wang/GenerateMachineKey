using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace GenerateMachineKey
{
    class Program
    {
        static void Main(string[] args)
        {
            SoftReg softReg = new SoftReg();
            //Console.WriteLine(softReg.getRNum());

            string mNum = softReg.getMNum();
            Console.WriteLine(mNum);

            SymmetricMethod method1 = new SymmetricMethod();
            string encrypStr = method1.Encrypto(mNum);
            Console.WriteLine(encrypStr);

            SymmetricMethod method2 = new SymmetricMethod();
            string decrypStr = method2.Decrypto(encrypStr);
            Console.WriteLine(decrypStr);
        }
    }
}

//.net 生成 机器码