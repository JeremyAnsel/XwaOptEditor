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
        public static string GetOpenOptFileName(string title, string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".opt",
                Filter = "OPT files (*.opt)|*.opt",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveOptFileName(string title, string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".opt",
                Filter = "OPT files (*.opt)|*.opt",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenObjFileName(string title, string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".obj",
                Filter = "OBJ files (*.obj)|*.obj",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveObjFileName(string title, string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".obj",
                Filter = "OBJ files (*.obj)|*.obj",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenRhinoFileName(string title, string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".3dm",
                Filter = "3DM files (*.3dm)|*.3dm",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveRhinoFileName(string title, string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".3dm",
                Filter = "3DM files (*.3dm)|*.3dm",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenAn8FileName(string title, string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".an8",
                Filter = "AN8 files (*.an8)|*.an8",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveAn8FileName(string title, string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".an8",
                Filter = "AN8 files (*.an8)|*.an8",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }

        public static string GetOpenTextureFileName(string title, string name)
        {
            var openMessage = new OpenFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".png",
                Filter = "Textures files (*.png, *.bmp, *.jpg, *.gif)|*.png;*.bmp;*.jpg;*.gif|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp|JPG files (*.jpg)|*.jpg|GIF files (*.gif)|*.gif",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(openMessage);

            return openMessage.FileName;
        }

        public static string GetSaveTextureFileName(string title, string name)
        {
            var saveMessage = new SaveFileDialogMessage
            {
                Title = title,
                DefaultExtension = ".png",
                Filter = "Textures (*.png, *.bmp)|*.png;*.bmp|PNG files (*.png)|*.png|BMP files (*.bmp)|*.bmp",
                FileName = string.IsNullOrEmpty(name) ? null : System.IO.Path.GetFileNameWithoutExtension(name)
            };

            Messenger.Instance.Notify(saveMessage);

            return saveMessage.FileName;
        }
    }
}
