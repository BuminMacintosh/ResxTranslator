using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace ResxTranslator.Translate
{
	public class Translator
	{
		public static string Translate(string sentence)
		{
			try
			{
				//Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
				AdmAuthentication admAuth = new AdmAuthentication(
					ConfigurationManager.AppSettings["datamarket.azure.com.application.cliendid"],
					ConfigurationManager.AppSettings["datamarket.azure.com.application.clientsecret"]
				);

				var url = string.Format(ConfigurationManager.AppSettings["api.microsofttranslator.urlTemplate"], sentence);
				var token = string.Format("Bearer {0}", admAuth.GetAccessToken().access_token);

				var translationWebRequest = WebRequest.Create(url);
				translationWebRequest.Headers.Add("Authorization", token);

				var webResponse = translationWebRequest.GetResponse();
				using (var translatedStream = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
				{
					var xTranslation = new XmlDocument();
					xTranslation.LoadXml(translatedStream.ReadToEnd());
					return xTranslation.InnerText;
				}
			}
			catch (Exception ex)
			{
				return ex.Message;
			}

		}
	}
}
