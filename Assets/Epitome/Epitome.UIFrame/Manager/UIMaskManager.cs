using UnityEngine;
using UnityEngine.UI;

namespace Epitome.UIFrame
{
    public class UIMaskManager : Singleton<UIMaskManager>
    {
        private UIMaskManager() { }

        private GameObject UIFrame_RootNode = null;

        private GameObject topPanel;
        private GameObject maskPanel;

        private Image maskImage;

        private Color[] maskColors;

        public override void OnSingletonInit()
        {
            UIFrame_RootNode = GameObject.Find(Defines.ROOTNODE);

            topPanel = UIFrame_RootNode;
            maskPanel = UIFrame_RootNode.transform.Find("PopUpNode/UIMaskPanel").gameObject;

            maskImage = maskPanel.GetComponent<Image>();

            maskColors = new Color[3];
            maskColors[0] = new Color(255 / 255F, 255 / 255F, 255 / 255F, 0F / 255F);
            maskColors[1] = new Color(0, 0, 0, 100 / 255F);
            maskColors[2] = new Color(0, 0, 0, 200F / 255F);

            base.OnSingletonInit();
        }

        public void SetMaskWindow(GameObject UIForms, UIMaskType maskType = UIMaskType.Lucency)
        {
            topPanel.transform.SetAsLastSibling();

            switch (maskType)
            {
                //完全透明，不能穿透
                case UIMaskType.Lucency:
                    maskPanel.SetActive(true);
                    maskImage.color = maskColors[0];
                    break;
                //半透明，不能穿透
                case UIMaskType.Translucence:
                    maskPanel.SetActive(true);
                    maskImage.color = maskColors[1];
                    break;
                //低透明，不能穿透
                case UIMaskType.ImPenetrable:
                    maskPanel.SetActive(true);
                    maskImage.color = maskColors[2];
                    break;
                //可以穿透
                case UIMaskType.Pentrate:
                    if (maskPanel.activeInHierarchy)
                        maskPanel.SetActive(false);
                    break;
                default:
                    break;
            }

            maskPanel.transform.SetAsLastSibling();

            UIForms.transform.SetAsLastSibling();
        }

        public void CancelMaskWindow()
        {
            topPanel.transform.SetAsFirstSibling();

            if (maskPanel.activeInHierarchy)
            {
                maskPanel.SetActive(false);
            }
        }
    }
}