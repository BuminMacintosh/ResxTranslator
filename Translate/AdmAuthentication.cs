﻿using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace ResxTranslator.Translate
{
	public class AdmAuthentication
	{
		public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
		private string clientId;
		private string cientSecret;
		private string request;

		public AdmAuthentication(string clientId, string clientSecret)
		{
			this.clientId = clientId;
			this.cientSecret = clientSecret;

			//If clientid or client secret has special characters, encode before sending request
			this.request = string.Format(
				"grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", 
				HttpUtility.UrlEncode(clientId),
				HttpUtility.UrlEncode(clientSecret)
			);
		}

		public AdmAccessToken GetAccessToken()
		{
			return HttpPost(DatamarketAccessUri, this.request);
		}

		private AdmAccessToken HttpPost(string DatamarketAccessUri, string requestDetails)
		{
			//Prepare OAuth request 
			var webRequest = WebRequest.Create(DatamarketAccessUri);
			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Method = "POST";
			var bytes = Encoding.ASCII.GetBytes(requestDetails);
			webRequest.ContentLength = bytes.Length;

			using (Stream outputStream = webRequest.GetRequestStream())
			{
				outputStream.Write(bytes, 0, bytes.Length);
			}

			using (WebResponse webResponse = webRequest.GetResponse())
			{
				var serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
				//Get deserialized object from JSON stream
				return (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
			}
		}
	}
}
