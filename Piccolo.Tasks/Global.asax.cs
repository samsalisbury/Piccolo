﻿using System;
using System.Web;

namespace Piccolo.Tasks
{
	public class Global : HttpApplication
	{
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			// TODO: Implement proper CORS support in Piccolo
			HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

			if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
			{
				//These headers are handling the "pre-flight" OPTIONS call sent by the browser
				HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
				HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
				HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
				HttpContext.Current.Response.End();
			}
		}
	}
}