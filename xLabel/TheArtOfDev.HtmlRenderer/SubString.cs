using System;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal sealed class SubString
	{
		private readonly string _fullString;

		private readonly int _startIdx;

		private readonly int _length;

		public string FullString
		{
			get
			{
				return _fullString;
			}
		}

		public int StartIdx
		{
			get
			{
				return _startIdx;
			}
		}

		public int Length
		{
			get
			{
				return _length;
			}
		}

		public char this[int idx]
		{
			get
			{
				if (idx < 0 || idx > _length)
					throw new ArgumentOutOfRangeException("idx", "must be within the string range");
				return _fullString[_startIdx + idx];
			}
		}

		public SubString(string fullString)
		{
			ArgChecker.AssertArgNotNull(fullString, "fullString");
			_fullString = fullString;
			_startIdx = 0;
			_length = fullString.Length;
		}

		public SubString(string fullString, int startIdx, int length)
		{
			ArgChecker.AssertArgNotNull(fullString, "fullString");
			if (startIdx < 0 || startIdx >= fullString.Length)
				throw new ArgumentOutOfRangeException("startIdx", "Must within fullString boundries");
			if (length < 0 || startIdx + length > fullString.Length)
				throw new ArgumentOutOfRangeException("length", "Must within fullString boundries");
			_fullString = fullString;
			_startIdx = startIdx;
			_length = length;
		}

		public bool IsEmpty()
		{
			return _length < 1;
		}

		public bool IsEmptyOrWhitespace()
		{
			for (int i = 0; i < _length; i++)
			{
				if (!char.IsWhiteSpace(_fullString, _startIdx + i))
					return false;
			}
			return true;
		}

		public bool IsWhitespace()
		{
			if (_length < 1)
				return false;
			for (int i = 0; i < _length; i++)
			{
				if (!char.IsWhiteSpace(_fullString, _startIdx + i))
					return false;
			}
			return true;
		}

		public string CutSubstring()
		{
			return (_length > 0) ? _fullString.Substring(_startIdx, _length) : string.Empty;
		}

		public string Substring(int startIdx, int length)
		{
			if (startIdx >= 0 && startIdx <= _length)
			{
				if (length <= _length)
				{
					if (startIdx + length > _length)
						throw new ArgumentOutOfRangeException("length");
					return _fullString.Substring(_startIdx + startIdx, length);
				}
				throw new ArgumentOutOfRangeException("length");
			}
			throw new ArgumentOutOfRangeException("startIdx");
		}

		public override string ToString()
		{
			return string.Format("Sub-string: {0}", (_length > 0) ? _fullString.Substring(_startIdx, _length) : string.Empty);
		}
	}
}
