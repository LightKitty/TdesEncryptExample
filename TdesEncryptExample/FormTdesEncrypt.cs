using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TdesEncryptExample
{
    public partial class FormTdesEncrypt : Form
    {
        public FormTdesEncrypt()
        {
            InitializeComponent();
        }

        private void FormTdesEncrypt_Load(object sender, EventArgs e)
        {
            textBoxEncrypt.ReadOnly = true;
            textBoxDecrypt.ReadOnly = true;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string str = textBoxInput.Text;
            if (str.Length == 0)
            {
                MessageBox.Show("请输入被加密的字符串");
                return;
            }
            //加密
            try
            {
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                //随机生成密钥Key 和初始化向量IV
                tdes.GenerateKey();
                tdes.GenerateIV();
                textBoxKey.Text = Encoding.UTF8.GetString(tdes.Key);
                //得到加密后的字节流
                byte[] encryptedBytes = EncryptText(str, tdes.Key, tdes.IV);
                //显示加密后的字符串
                textBoxEncrypt.Text = Encoding.UTF8.GetString(encryptedBytes);
                //解密
                string decryptString = DecryptText(encryptedBytes, tdes.Key, tdes.IV);
                //显示解密后的字符串
                textBoxDecrypt.Text = decryptString;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "出错");
            }
        }

        private byte[] EncryptText(string str, byte[] Key, byte[] IV)
        {
            //创建一个内存流
            MemoryStream memoryStream = new MemoryStream();
            //使用传递的私钥和IV 创建加密流
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
            new TripleDESCryptoServiceProvider().CreateEncryptor(Key, IV),
            CryptoStreamMode.Write);
            //将传递的字符串转换为字节数组
            byte[] toEncrypt = Encoding.UTF8.GetBytes(str);
            try
            {
                //将字节数组写入加密流,并清除缓冲区
                cryptoStream.Write(toEncrypt, 0, toEncrypt.Length);
                cryptoStream.FlushFinalBlock();
                //得到加密后的字节数组
                byte[] encryptedBytes = memoryStream.ToArray();
                return encryptedBytes;
            }
            catch (CryptographicException err)
            {
                throw new Exception("加密出错：" + err.Message);
            }
            finally
            {
                cryptoStream.Close();
                memoryStream.Close();
            }
        }

        private string DecryptText(byte[] dataBytes, byte[] Key, byte[] IV)
        {
            //根据加密后的字节数组创建一个内存流
            MemoryStream memoryStream = new MemoryStream(dataBytes);
            //使用传递的私钥、IV 和内存流创建解密流
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
            new TripleDESCryptoServiceProvider().CreateDecryptor(Key, IV),
            CryptoStreamMode.Read);
            //创建一个字节数组保存解密后的数据
            byte[] decryptBytes = new byte[dataBytes.Length];
            try
            {
                //从解密流中将解密后的数据读到字节数组中
                cryptoStream.Read(decryptBytes, 0, decryptBytes.Length);
                //得到解密后的字符串
                string decryptedString = Encoding.UTF8.GetString(decryptBytes);
                return decryptedString;
            }
            catch (CryptographicException err)
            {
                throw new Exception("解密出错：" + err.Message);
            }
            finally
            {
                cryptoStream.Close();
                memoryStream.Close();
            }
        }
    }
}

