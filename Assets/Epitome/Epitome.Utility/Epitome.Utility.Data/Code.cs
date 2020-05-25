/*----------------------------------------------------------------
 * 文件名：Code
 * 文件功能描述：码
----------------------------------------------------------------*/
using UnityEngine;  
using System.Collections;
using System;
using System.IO;
using Epitome.Manager;
using ZXing;
using ZXing.QrCode;

namespace Epitome.Utility
{
    public class Code
    {
        static Code mInstance;

        public static Code GetSingleton() { if (mInstance == null) { mInstance = new Code(); } return mInstance; }

        //++++++++++++++++++++     二维码     ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 生成二维码.
        /// </summary>
        public void GenerateQRCode(string varStr,string varName="", int varPixels = 256, float varInt = 6f)
        {
            if (varStr != null)
            {
                //生成二维码
                Texture2D tempTexture2D = new Texture2D(varPixels, varPixels);
                var tempColor32 = Encode(varStr, tempTexture2D.width, tempTexture2D.height);
                tempTexture2D.SetPixels32(tempColor32);
                //裁剪多余空白区域
                int tempInt = (int)(varPixels / varInt);
                var tempColor = tempTexture2D.GetPixels(tempInt/2, tempInt/2, varPixels- tempInt, varPixels - tempInt);
                tempColor32 = new Color32[tempColor.Length];
                for (int j = 0; j < tempColor.Length; j++) { tempColor32[j] = tempColor[j]; }
                tempTexture2D = new Texture2D(varPixels - tempInt, varPixels - tempInt);
                tempTexture2D.SetPixels32(tempColor32);
                //保存图片
                byte[] bytes = tempTexture2D.EncodeToPNG();
                Project.CreateDirectory(Application.dataPath + "/AdamBieber");
                string fileName;
                if (varName == "")
                    fileName = Application.dataPath + "/AdamBieber/" + varStr + ".png";
                else
                    fileName = Application.dataPath + "/AdamBieber/" + varName + ".png";
                File.WriteAllBytes(fileName, bytes);
            }
        }

        private static Color32[] Encode(string varStr, int varWidth, int varHeight)
        {
            var writer = new BarcodeWriter{Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions{Height = varHeight,Width = varWidth}};
            return writer.Write(varStr);
        }


        /// <summary>
        /// 识别二维码 
        /// </summary>
        public IEnumerator IdentifyQRCode(Color32[] varColor32,string varStr)
        {
            yield return new WaitForEndOfFrame();

            BarcodeReader tempBarcodeReader = new BarcodeReader();

            var data = tempBarcodeReader.Decode(varColor32, (int)Mathf.Sqrt(varColor32.Length), (int)Mathf.Sqrt(varColor32.Length));
            if (data != null)
            {
                switch (data.BarcodeFormat)
                {
                    case BarcodeFormat.QR_CODE:
                        break;
                }
                EventManager.Instance.BroadcastEvent(varStr, data);
            }
        }

        /// <summary>
        /// 识别二维码 
        /// </summary>
        public IEnumerator IdentifyQRCode()
        {
            yield return new WaitForEndOfFrame();

            BarcodeReader tempBarcodeReader = new BarcodeReader();

            Texture2D tempTxteure = new Texture2D(300, 300);
            tempTxteure.ReadPixels(new Rect(0, 0, 300, 300), 0, 0);
            tempTxteure.Apply();

            Color[] tempColor = tempTxteure.GetPixels();
            Color32[] tempColor32 = new Color32[tempColor.Length];
            for (int j = 0; j < tempColor.Length; j++) { tempColor32[j] = tempColor[j]; }

            var data = tempBarcodeReader.Decode(tempColor32, 300, 300);
            if (data != null)
            {
                switch (data.BarcodeFormat)
                {
                    case BarcodeFormat.QR_CODE:
                        break;
                }
                Debug.Log(data);
            }
        }
    }
}