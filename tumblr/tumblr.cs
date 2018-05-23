using System;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace tumblr
{
    public class tumblr
    {
        public class Account
        {
            private string sEmail = "";
            private string sPassword = "";
            private string sGroup = "";
            private string sDate = "";
            private string sTags = "";

            public Account()
            {
            }

            public string Email
            {
                get
                {
                    return sEmail;
                }
                set
                {
                    sEmail = value;
                }
            }
            public string Password
            {
                get
                {
                    return sPassword;
                }
                set
                {
                    sPassword = value;
                }
            }
            public string Group
            {
                get
                {
                    return sGroup;
                }
                set
                {
                    sGroup = value;
                }
            }

            public bool IsPrivate { get; set; }

            public DateTime DateOfPost
            {
                get
                {
                    return Convert.ToDateTime(sDate);
                }
                set
                {
                    if (value > DateTime.Now)
                    {
                        sDate = "";
                    }
                    else
                    {
                        sDate = value.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            public string StrDateOfPost
            {
                get
                {
                    return sDate;
                }
                set
                {
                    sDate = value;
                }
            }
            public string Tags
            {
                get
                {
                    return sTags;
                }
                set
                {
                    sTags = value;
                }
            }
        }
        public class Text : Account
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Body { get; set; }
        }

        public class Photo : Account
        {
            // This class will convert a file into a URL-encoded string
            // and store the string in sData. I thought of doing it in the  
            // btnPost_Click event, however I felt it would be a better 
            // design decision to have it as part of the Photo object.
            private string sSource = "";
            private string sData = "";
            private string sCaption = "";
            private string sClickThroughUrl = "";
            public string Source 
            {
                get 
                {
                    return sSource; 
                }
                set 
                {
                    sSource = value; 
                }
            }
            public string Data
            {
                get 
                {
                    return sData; 
                }
                // Check whether the file exists and stream it-->url-encode it
                set 
                {
                    if (value.Length > 0)
                    {
                        if (System.IO.File.Exists(value))
                        {
                            try
                            {
                                byte[] whole = System.IO.File.ReadAllBytes(value);
                                sData = System.Web.HttpUtility.UrlEncode(whole);
                            }
                            catch (Exception e)
                            {
                                // Let the user know what went wrong.
                                throw e;
                            }
                        }
                        else
                        {
                            throw new Exception("There was something wrong with the File you selected. Please verify that the file exists.");

                        }
                    }
                }
            }
            public string Caption
            {
                get
                {
                    return sCaption;
                }
                set
                {
                    string s = value.Replace("\r", "");
                    s = s.Replace("\n", "");
                    sCaption = System.Web.HttpUtility.UrlEncode(s, Encoding.UTF8);
                }
            }
            public string ClickThroughUrl
            {
                get
                {
                    return sClickThroughUrl;
                }
                set
                {
                    sClickThroughUrl = value;
                }
            }
        }

        public class Quote : Account
        {
            private string sQuote = "";
            private string sSource = "";
            public string TheQuote
            {
                get
                {
                    return sQuote;
                }
                set
                {
                    sQuote = System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8);
                }
            }
            public string Source
            {
                get
                {
                    return sSource;
                }
                set
                {
                    string s = value.Replace("\r", "");
                    s = s.Replace("\n", "");
                    sSource = System.Web.HttpUtility.UrlEncode(s, Encoding.UTF8);
                }
            }
        }

        public class Link : Account
        {
            private string sName = "";
            private string sUrl = "";
            private string sDescription = "";
            public string Name
            {
                get
                {
                    return sName;
                }
                set
                {
                    sName = System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8);
                }
            }
            public string Url
            {
                get
                {
                    return sUrl;
                }
                set
                {
                    sUrl = value;
                }
            }
            public string Description
            {
                get
                {
                    return sDescription;
                }
                set
                {
                    string s = value.Replace("\r", "");
                    s = s.Replace("\n", "");
                    sDescription = System.Web.HttpUtility.UrlEncode(s, Encoding.UTF8);
                }
            }
        }

        public class Chat : Account
        {
            private string sTitle = "";
            private string sChat = "";
            public string Title
            {
                get
                {
                    return sTitle;
                }
                set
                {
                    sTitle = System.Web.HttpUtility.UrlEncode(value, Encoding.UTF8);
                }
            }
            public string TheChat
            {
                get
                {
                    return sChat;
                }
                set
                {
                    sChat = HttpUtility.UrlEncode(value, Encoding.UTF8);
                }
            }
        }

        public class Video : Account
        {
            private string sEmbed = "";
            private string sCaption = "";
            public string Embed
            {
                get
                {
                    return sEmbed;
                }
                set
                {
                    sEmbed = value;
                }
            }
            public string Caption
            {
                get
                {
                    return sCaption;
                }
                set
                {
                    string s = value.Replace("\r", "");
                    s = s.Replace("\n", "");
                    sCaption = HttpUtility.UrlEncode(s, Encoding.UTF8);
                }
            }
        }

        private const string generator = "http://www.tradelr.com";

        public Status deleteText(Text text)
        {
            Encoder enc = Encoding.UTF8.GetEncoder();
            string postData = "email=" + text.Email;
            postData += "&password=" + text.Password;
            postData += "&post-id=" + text.Id;
            char[] chr = postData.ToCharArray();
            int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
            byte[] data = new Byte[byteCount];
            return DeleteIt(data);
        }

        private Status DeleteIt(byte[] data)
        {
            // Generic Post to Tumblr method            
            HttpWebResponse response = null;
            Stream newStream = null;
            Status status = new Status();
            // Prepare web request...
            var myRequest = (HttpWebRequest)WebRequest.Create("http://www.tumblr.com/api/delete");
            myRequest.Credentials = CredentialCache.DefaultCredentials;
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            myRequest.Timeout = 600000;
            myRequest.ContentLength = data.Length;
            try
            {
                newStream = myRequest.GetRequestStream();
                // Send the data.
                newStream.WriteTimeout = 600000; // Ten minutes - Change made due to timeout on larger uploads. I wonder which is enforced? myRequest or the newStream.
                newStream.Write(data, 0, data.Length);
                // Get the response
                response = (HttpWebResponse)myRequest.GetResponse();
                status.Code = Convert.ToInt32(response.StatusCode);
                status.Msg = response.StatusDescription;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    status.Id = sr.ReadToEnd();
                }
            }
            catch (ProtocolViolationException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (WebException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (Exception e)
            {
                status.Msg = Convert.ToString(e.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (newStream != null)
                {
                    newStream.Close();
                }
            }
            return status;
        }

        private Status PostIt(byte[] data)
        {
            // Generic Post to Tumblr method            
            HttpWebResponse response = null;
            Stream newStream = null;
            Status status = new Status();
            // Prepare web request...
            var myRequest = (HttpWebRequest)WebRequest.Create("http://www.tumblr.com/api/write");
            myRequest.Credentials = CredentialCache.DefaultCredentials;
            myRequest.Method = "POST";
            myRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            myRequest.ReadWriteTimeout = 600000; // Ten minutes - default should be 5 minutes. Change made due to timeout on larger uploads
            myRequest.Timeout = 600000;
            //header("Content-Type: text/html; charset=iso-8859-1");            
            myRequest.ContentLength = data.Length;
            try
            {
                newStream = myRequest.GetRequestStream();
                // Send the data.
                newStream.WriteTimeout = 600000; // Ten minutes - Change made due to timeout on larger uploads. I wonder which is enforced? myRequest or the newStream.
                newStream.Write(data, 0, data.Length);
                // Get the response
                response = (HttpWebResponse)myRequest.GetResponse();
                status.Code = Convert.ToInt32(response.StatusCode);
                status.Msg = response.StatusDescription;
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    status.Id = sr.ReadToEnd();
                }
            }
            catch (ProtocolViolationException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (WebException ex)
            {
                status.Msg = Convert.ToString(ex.Message);
            }
            catch (Exception e)
            {
                status.Msg = Convert.ToString(e.Message);
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (newStream != null)
                {
                    newStream.Close();
                }
            }
            return status;
        }

        public Status postText(Text text)
        {
            Encoder enc = Encoding.UTF8.GetEncoder();
            string postData = "email=" + text.Email;
            postData += "&password=" + text.Password;
            if (text.Group.Length > 0)
            {
                postData += "&group=" + text.Group;
            }
            postData += "&type=regular";
            if (string.IsNullOrEmpty(text.Title) && string.IsNullOrEmpty(text.Body))
            {
                throw new ArgumentNullException();
            }

            if (!string.IsNullOrEmpty(text.Title))
            {
                postData += "&title=" + text.Title;
            }
            if (!string.IsNullOrEmpty(text.Body))
            {
                postData += "&body=" + text.Body;
            }
            postData += "&generator=" + generator;
            if (text.StrDateOfPost.Length > 0)
            {
                postData += "&date=" + text.StrDateOfPost;
            }
            if (text.IsPrivate)
            {
                postData += "&private=1";
            }
            if (text.Tags.Length > 0)
            {
                postData += "&tags=" + text.Tags;
            }
            if (!string.IsNullOrEmpty(text.Id))
            {
                postData += "&post-id=" + text.Id;
            }

            char[] chr = postData.ToCharArray();
            int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
            byte[] data = new Byte[byteCount];
            int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
            return PostIt(data);
         }

        public Status postPhoto(Photo photo)
        {
            Encoder enc = Encoding.UTF8.GetEncoder();
            string postData = "email=" + photo.Email;
            postData += "&password=" + photo.Password;
            if (photo.Group.Length > 0)
            {
                postData += "&group=" + photo.Group;
            }
            postData += "&type=photo";
            if (string.IsNullOrEmpty(photo.Source) && ((photo.Data == null) || (photo.Data == "")))
            {
                throw new ArgumentNullException();
            }
            else
            {
                if (!string.IsNullOrEmpty(photo.Source))
                {
                    postData += "&source=" + photo.Source;
                }
                else // Don't bother sending a picture if the source is here
                {
                    if (!string.IsNullOrEmpty(photo.Data))
                    {
                        postData += "&data=" + photo.Data;
                    }
                }
                if (!string.IsNullOrEmpty(photo.Caption))
                {
                    postData += "&caption=" + photo.Caption;
                }
                if (!string.IsNullOrEmpty(photo.ClickThroughUrl))
                {
                    postData += "&click-through-url=" + photo.ClickThroughUrl;
                }
            }
            postData += "&generator=" + generator;
            if (photo.StrDateOfPost.Length > 0)
            {
                postData += "&date=" + photo.StrDateOfPost;
            }
            if (photo.IsPrivate)
            {
                postData += "&private=1";
            }
            if (photo.Tags.Length > 0)
            {
                postData += "&tags=" + photo.Tags;
            }

            char[] chr = postData.ToCharArray();
            int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
            byte[] data = new Byte[byteCount];
            int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
            return PostIt(data);
        }

        public Status postQuote(Quote quote)
        {
            Encoder enc = Encoding.UTF8.GetEncoder();
            string postData = "email=" + quote.Email;
            postData += "&password=" + quote.Password;
            if (quote.Group.Length > 0)
            {
                postData += "&group=" + quote.Group;
            }
            postData += "&type=quote";
            if (string.IsNullOrEmpty(quote.TheQuote))
            {
                throw new ArgumentNullException();
            }
            else
            {
                if ((quote.TheQuote != null) && (quote.TheQuote != ""))
                {
                    postData += "&quote=" + quote.TheQuote;
                }
                if (!string.IsNullOrEmpty(quote.Source))
                {
                    postData += "&source=" + quote.Source;
                }
            }
            postData += "&generator=" + generator;
            if (quote.StrDateOfPost.Length > 0)
            {
                postData += "&date=" + quote.StrDateOfPost;
            }
            if (quote.IsPrivate)
            {
                postData += "&private=1";
            }
            if (quote.Tags.Length > 0)
            {
                postData += "&tags=" + quote.Tags;
            }

            char[] chr = postData.ToCharArray();
            int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
            byte[] data = new Byte[byteCount];
            int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
            return PostIt(data);
         }

        public Status postLink(Link link)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + link.Email;
             postData += "&password=" + link.Password;
             if (link.Group.Length > 0)
             {
                 postData += "&group=" + link.Group;
             }
             postData += "&type=link";
             if ((link.Url == null) || (link.Url == ""))
             {
                 throw new ArgumentNullException();
             }
             else
             {
                 if ((link.Name != null) && (link.Name != ""))
                 {
                     postData += "&name=" + link.Name;
                 }
                 if ((link.Url != null) && (link.Url != ""))
                 {
                     postData += "&url=" + link.Url;
                 }
                 if ((link.Description != null) && (link.Description != ""))
                 {
                     postData += "&description=" + link.Description;
                 }
             }
             postData += "&generator=" + generator;
             if (link.StrDateOfPost.Length > 0)
             {
                 postData += "&date=" + link.StrDateOfPost;
             }
             if (link.IsPrivate)
             {
                 postData += "&private=1";
             }
             if (link.Tags.Length > 0)
             {
                 postData += "&tags=" + link.Tags;
             }

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        public Status postChat(Chat chat)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + chat.Email;
             postData += "&password=" + chat.Password;
             if (chat.Group.Length > 0)
             {
                 postData += "&group=" + chat.Group;
             }
             postData += "&type=conversation";
             if ((chat.TheChat == null) || (chat.TheChat == ""))
             {
                 throw new ArgumentNullException();
             }
             else
             {
                 if ((chat.Title != null) && (chat.Title != ""))
                 {
                     postData += "&title=" + chat.Title;
                 }
                 if ((chat.TheChat != null) && (chat.TheChat != ""))
                 {
                     postData += "&conversation=" + chat.TheChat;
                 }
             }
             postData += "&generator=" + generator;
             if (chat.StrDateOfPost.Length > 0)
             {
                 postData += "&date=" + chat.StrDateOfPost;
             }
             if (chat.IsPrivate)
             {
                 postData += "&private=1";
             }
             if (chat.Tags.Length > 0)
             {
                 postData += "&tags=" + chat.Tags;
             }

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        public Status postVideo(Video video)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + video.Email;
             postData += "&password=" + video.Password;
             if (video.Group.Length > 0)
             {
                 postData += "&group=" + video.Group;
             }
             postData += "&type=video";
             if ((video.Embed == null) || (video.Embed == ""))
             {
                 throw new ArgumentNullException();
             }
             else
             {
                 if ((video.Embed != null) && (video.Embed != ""))
                 {
                     postData += "&embed=" + video.Embed;
                 }
                 if ((video.Caption != null) && (video.Caption != ""))
                 {
                     postData += "&caption=" + video.Caption;
                 }
             }
             postData += "&generator=" + generator;
             if (video.StrDateOfPost.Length > 0)
             {
                 postData += "&date=" + video.StrDateOfPost;
             }
             if (video.IsPrivate)
             {
                 postData += "&private=1";
             }
             if (video.Tags.Length > 0)
             {
                 postData += "&tags=" + video.Tags;
             }

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        public Status Authenticate(Account account)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + account.Email;
             postData += "&password=" + account.Password;
             postData += "&action=authenticate";
             postData += "&generator=" + generator;

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        public Status CheckVimeo(Account account)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + account.Email;
             postData += "&password=" + account.Password;
             postData += "&action=check-vimeo";
             postData += "&generator=" + generator;

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        public Status CheckAudio(Account account)
         {
             Encoder enc = Encoding.UTF8.GetEncoder();
             string postData = "email=" + account.Email;
             postData += "&password=" + account.Password;
             postData += "&action=check-audio";
             postData += "&generator=" + generator;

             char[] chr = postData.ToCharArray();
             int byteCount = enc.GetByteCount(chr, 0, chr.Length, true);
             byte[] data = new Byte[byteCount];
             int bytesEncodedCount = enc.GetBytes(chr, 0, chr.Length, data, 0, true);
             return PostIt(data);
         }

        
    }
}
