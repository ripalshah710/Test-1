using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using RestSharp;
using Newtonsoft.Json.Linq;
using Queue.Models;
using Newtonsoft.Json;

namespace CanvasAWSHandler
{
    public class AWSApiHandler
    {
		private string _api = null;
		public string api
		{
			set { this._api = value; }
			get { return this._api; }
		}
		private string _account_id = null;
		public string account_id
		{
			set { this._account_id = value; }
			get { return this._account_id; }
		}
		private string _client_id = null;
		public string client_id
		{
			set { this._client_id = value; }
			get { return this._client_id; }
		}
		private string _client_secret = null;
		public string client_secret
		{
			set { this._client_secret = value; }
			get { return this._client_secret; }
		}
		private string _redirect_uri = null;
		public string redirect_uri
		{
			set { this._redirect_uri = value; }
			get { return this._redirect_uri; }
		}
		private string _code = null;
		public string code
		{
			set { this._code = value; }
			get { return this._code; }
		}
		private string _sub_domain = null;
		public string sub_domain
		{
			set { this._sub_domain = value; }
			get { return this._sub_domain; }
		}

		private string _access_token = null;
		public string access_token
		{
			set { this._access_token = value; }
			get { return this._access_token; }
		}



		private bool validateSettings()
		{
			if (code != null && client_id != null && client_secret != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public string GetApiData(string _sub_domain,string _api, string _access_token, string keyword="")
		{
			string api = _api;
			string access_token = _access_token;
			string sub_domain = _sub_domain;
			if(keyword!="")
			{
				keyword = "&" + keyword;
			}

			string url = @"https://" + sub_domain + ".instructure.com/"+api+"?access_token="+access_token+ keyword;

			return Get(url);
		}
		public string Get(string uri)
		{
			try
			{
				System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
				request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				request.MaximumAutomaticRedirections = 4;
				request.MaximumResponseHeadersLength = 4;
				// Set credentials to use for this request.
				request.Credentials = CredentialCache.DefaultCredentials;
				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					var data = reader.ReadToEnd();
					response.Close();
					stream.Close();
					return data;
				}
			}
			catch (Exception ex)
			{
				return "[{\"err\":\"" + ex.Message + "}]";
			}

		}

		public Hashtable PostApiData(string _sub_domain, string _api, string _access_token,string data)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			sub_domain = _sub_domain;
			api = _api;
			access_token = _access_token;

			if (validateSettings())
			{
				// TODO I know null isn't the best response for this return type but if the settings aren't valid it shouldn't
				// work.  What is a better return type?
				//return null;
			}
			else
			{

				//	string url = _api + "?access_token=" + _access_token;
				string url = _api + "?authorization=Bearer " + _access_token;


				//	string url = @"https://publishercanvas.escworks.app/Publisher";//@"http://localhost:20756/Publisher";

				// "https://localhost:44336/api/canvas/user/enroll";

				WebClient client = new WebClient();
				client.Encoding = Encoding.UTF8;
				string method = "POST";
				try
				{
					client.Headers[HttpRequestHeader.ContentType] = "application/json";				
					
					client.Headers["Authorization"] = "Bearer "+ _access_token;
					


					var response = client.UploadString(url, method, data); //4

					client.Dispose();

					//return response;
					Hashtable json_response = returnJson(response); //5
					json_response["raw_response"] = response;
					json_response["status_code"] = "ok";
					json_response["response"] = "completed successfully.";
					return json_response; //6

				}

				catch (Exception ex)
				{
					Hashtable json_response = new Hashtable();
					json_response["status_code"] = "error";
					json_response["response"] = ex.Message;
					return json_response;
				}

			}
			return null;
		}

		public Hashtable CheckStatus(String sis_import_id)
		{
			System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			//string url = @"https://" + sub_domain + ".instructure.com/api/v1/accounts/82726/sis_imports/"+sis_import_id +".json?access_token=<access_token>";
			string url = @"https://" + sub_domain + ".instructure.com/api/v1/accounts/" + account_id + "/sis_imports/" + sis_import_id + ".json?access_token=" + access_token;

			NameValueCollection nvc = new NameValueCollection();

			nvc.Add("access_token", access_token);
			WebClient client = new WebClient();
			client.QueryString = nvc;
			byte[] responseBinary = client.DownloadData(url);
			string response = Encoding.UTF8.GetString(responseBinary);

			//return response;
			Hashtable json_response = returnJson(response);
			json_response["raw_response"] = response;
			return json_response;

		}

		private Hashtable returnJson(string s)
		{
			Hashtable o;
			//bool success = true;
			o = (Hashtable)JSON.JsonDecode(s);
			//Console.WriteLine (o["id"]);
			return o;
		}

		// REST CSharp

		public string AWSPost(string api, string jason_data, string Aud, string customer_id)
		{

			string url = "https://canvas-api-auth.auth.us-east-1.amazoncognito.com/oauth2/token"; //1
			String id = "6aprqq1hisp70a4jep3dk86fke";
			String secret = "od42icspdvkqnl2120994fdhfdlpgn4fb7ti1bmnk57m64vfc7f";

			var _client = new RestClient(url);
			var _request = new RestRequest(Method.POST);
			_request.AddHeader("cache-control", "no-cache");
			_request.AddHeader("content-type", "application/x-www-form-urlencoded");
			_request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&scope=canvas-user/add&client_id=" + id + "&client_secret=" + secret, ParameterType.RequestBody);
			IRestResponse _response = _client.Execute(_request);

			dynamic resp = JObject.Parse(_response.Content);
			String _token = resp.access_token; //2
			

		

			//var jason_data = CourseEnrollment(R4session.ID.ToString(), User);

			var Msg = new Message() { Topic = "CanvasPublisherQueue", isTopicCreated = true, Msg = jason_data, Aud = Aud, CustomerID= customer_id };

			var data = QueueJson(Msg);





			//AWSApiHandler apiHandler = new AWSApiHandler();
			var retUpdate = PostApiData("", api, _token, data); //3


			return string.Empty;


		}


		public string QueueJson(Message _Msg)
		{
			Message Msg = new Message() { Topic = _Msg.Topic, isTopicCreated = _Msg.isTopicCreated, Msg = _Msg.Msg, Aud = _Msg.Aud };
			return JsonConvert.SerializeObject(Msg);
		}




		public Hashtable AWSApiPost(string _api, string jason_data, string Aud, string customer_id)
		{
			if (validateSettings())
			{
				// TODO I know null isn't the best response for this return type but if the settings aren't valid it shouldn't
				// work.  What is a better return type?
				return null;
			}
			else
			{
				string url = "https://canvas-api-auth.auth.us-east-1.amazoncognito.com/oauth2/token";
				String id = "6aprqq1hisp70a4jep3dk86fke";
				String secret = "od42icspdvkqnl2120994fdhfdlpgn4fb7ti1bmnk57m64vfc7f";

				NameValueCollection nvc = new NameValueCollection();

				nvc.Add("client_id", id);
				nvc.Add("client_secret", secret);
				nvc.Add("grant_type", "client_credentials");
				nvc.Add("scope", "canvas-user/add ");


				WebClient client = new WebClient();

				client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var responseValues = client.UploadValues(url, "POST", nvc);

				string response = Encoding.UTF8.GetString(responseValues);
				dynamic resp = JObject.Parse(response);
				String _token = resp.access_token; //aws cognito token
				var Msg = new Message() { Topic = "CanvasPublisherQueue", isTopicCreated = true, Msg = jason_data, Aud = Aud, CustomerID = customer_id };

				var data = QueueJson(Msg);

				
				var retUpdate = PostApiData("", _api, _token, data);
				return retUpdate;
			}
		}



	}
}

