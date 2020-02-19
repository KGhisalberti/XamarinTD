using System;
using System.Collections.Generic;
using System.Text;

namespace TD2 {
	public static class Urls {
		public const string URI = "https://td-api.julienmialon.com/";
		public const string LOGIN = "auth/login"; // POST login with email/password
		public const string REFRESH = "auth/refresh"; // POST refresh a token
		public const string REGISTER = "auth/register"; // POST register a user
		public const string ME = "me"; // GET User profile
		public const string UPDATE_PASSWORD = "me/password"; // PATCH Update password

		public const string PLACES = "places";
		public const string COMMENT = "/comments";
		public const string IMAGE = "images";
	}

	public static class Errors {
		public const string IMAGE_NOT_FOUND = nameof(IMAGE_NOT_FOUND);

		public const string EMAIL_ALREADY_EXISTS = nameof(EMAIL_ALREADY_EXISTS);
	}
}
