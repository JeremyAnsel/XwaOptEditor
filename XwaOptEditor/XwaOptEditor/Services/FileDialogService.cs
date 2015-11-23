using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XwaOptEditor.Messages;
using XwaOptEditor.Mvvm;

namespace XwaOptEditor.Services
{
    static class FileDialogService
    {
        public static string GetOpenOptFileName()
        {
            return FileDialogService.GetOpenOptFileName(null);
        }

        public static string GetOpenOptFileName(string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                DefaultExtension = ".opt",
                Filter = "OPT files (*.opt)|*.opt",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveOptFileName()
        {
            return FileDialogService.GetSaveOptFileName(null);
        }

        public static string GetSaveOptFileName(string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                DefaultExtension = ".opt",
                Filter = "OPT files (*.opt)|*.opt",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenObjFileName()
        {
            return FileDialogService.GetOpenObjFileName(null);
        }

        public static string GetOpenObjFileName(string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                DefaultExtension = ".obj",
                Filter = "OBJ files (*.obj)|*.obj",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveObjFileName()
        {
            return FileDialogService.GetSaveObjFileName(null);
        }

        public static string GetSaveObjFileName(string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                DefaultExtension = ".obj",
                Filter = "OBJ files (*.obj)|*.obj",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenRhinoFileName()
        {
            return FileDialogService.GetOpenRhinoFileName(null);
        }

        public static string GetOpenRhinoFileName(string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                DefaultExtension = ".3dm",
                Filter = "3DM files (*.3dm)|*.3dm",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveRhinoFileName()
        {
            return FileDialogService.GetSaveRhinoFileName(null);
        }

        public static string GetSaveRhinoFileName(string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                DefaultExtension = ".3dm",
                Filter = "3DM files (*.3dm)|*.3dm",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenAn8FileName()
        {
            return FileDialogService.GetOpenAn8FileName(null);
        }

        public static string GetOpenAn8FileName(string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                DefaultExtension = ".an8",
                Filter = "AN8 files (*.an8)|*.an8",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveAn8FileName()
        {
            return FileDialogService.GetSaveAn8FileName(null);
        }

        public static string GetSaveAn8FileName(string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                DefaultExtension = ".an8",
                Filter = "AN8 files (*.an8)|*.an8",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenTextureFileName()
        {
            return FileDialogService.GetOpenTextureFileName(null);
        }

        public static string GetOpenTextureFileName(string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                DefaultExtension = ".png",
                Filter = "Textures files (*.png, *.bmp, *.jpg, *.gif)|*.png;*.bmp;*.jpg;*.gif|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|JPG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveTextureFileName()
        {
            return FileDialogService.GetSaveTextureFileName(null);
        }

        public static string GetSaveTextureFileName(string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                DefaultExtension = ".png",
                Filter = "Textures (*.png, *.bmp)|*.png;*.bmp|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }
    }
}
