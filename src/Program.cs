using System.Linq;

namespace CharCmp {

	public static class Program {

		[System.STAThread]
		public static System.Int32 Main( System.String[] args ) {
			var output = 2;

			if ( null == args ) {
				PrintUsage();
				return output;
			}

			var len = args.Length;
			if ( ( len < 4 ) || ( 6 < len ) || ( 0 != ( len % 2 ) ) ) {
				PrintUsage();
				return output;
			}

			System.String left = null;
			System.String right = null;
			System.String cp = "windows-1252";
			var i = 0;
			do {
				if ( args[ i ].Equals( "/fileA", System.StringComparison.OrdinalIgnoreCase ) ) {
					left = args[ ++i ].Trim();
				} else if ( args[ i ].Equals( "/fileB", System.StringComparison.OrdinalIgnoreCase ) ) {
					right = args[ ++i ].Trim();
				} else if ( args[ i ].Equals( "/cp", System.StringComparison.OrdinalIgnoreCase ) ) {
					cp = args[ ++i ].Trim();
				} else {
					PrintUsage();
					return output;
				}
			} while ( ++i < len );
			if ( System.String.IsNullOrEmpty( left ) || System.String.IsNullOrEmpty( right ) ) {
				PrintUsage();
				return output;
			}

			System.Text.Encoding encoding = System.Text.Encoding.GetEncoding( cp );
			if ( null == encoding ) {
				System.Int32 cpn;
				if ( System.Int32.TryParse( cp, out cpn ) ) {
					encoding = System.Text.Encoding.GetEncoding( cpn );
				}
			}
			if ( null == encoding ) {
				PrintUnkCodePage();
				return output;
			}

			output = IsSameByCharValue( left, right, encoding )
				? 0
				: 1
			;

			return output;
		}

		private static System.Boolean IsSameByCharValue( System.String left, System.String right, System.Text.Encoding encoding ) {
#if TRACE || DEBUG
			if ( null == right ) {
				throw new System.ArgumentNullException( "right" );
			} else if ( null == left ) {
				throw new System.ArgumentNullException( "left" );
			} else if ( null == encoding ) {
				throw new System.ArgumentNullException( "encoding" );
			}
#endif
			using ( var leftF = System.IO.File.Open( left, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
				using ( var rightF = System.IO.File.Open( right, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read ) ) {
					return IsSameByCharValue( leftF, rightF, encoding );
				}
			}
		}

		private static System.Boolean IsSameByCharValue( System.IO.Stream left, System.IO.Stream right, System.Text.Encoding encoding ) {
#if TRACE || DEBUG
			if ( null == right ) {
				throw new System.ArgumentNullException( "right" );
			} else if ( null == left ) {
				throw new System.ArgumentNullException( "left" );
			} else if ( null == encoding ) {
				throw new System.ArgumentNullException( "encoding" );
			}
#endif
			using ( var leftR = new System.IO.StreamReader( left, encoding, true, 16384, true ) ) {
				using ( var rightR = new System.IO.StreamReader( right, encoding, true, 16384, true ) ) {
					return IsSameByCharValue( leftR, rightR );
				}
			}
		}
		private static System.Boolean IsSameByCharValue( System.IO.StreamReader left, System.IO.StreamReader right ) {
#if TRACE || DEBUG
			if ( null == right ) {
				throw new System.ArgumentNullException( "right" );
			} else if ( null == left ) {
				throw new System.ArgumentNullException( "left" );
			}
#endif
			var output = true;
			System.String l;
			System.String r;
			System.Boolean reading = true;
			do {
				l = left.ReadLine();
				r = right.ReadLine();
				if ( !IsSameByCharValue( l, r ) ) {
					output = false;
					reading = false;
				} else if ( ( null == l ) || ( null == r ) ) {
					reading = false;
				}
			} while ( reading );
			return output;
		}
		private static System.Boolean IsSameByCharValue( System.String left, System.String right ) {
			return ( ( null == left ) && ( null == right ) )
				? true
				: ( ( null == left ) != ( null == right ) )
					? false
					: System.String.Equals( left.Normalize(), right.Normalize(), System.StringComparison.Ordinal )
			;
		}


		private static void PrintUsage() {
			var err = System.Console.Error;
			err.WriteLine( "No! No! No! Use it like this, Einstein:" );
			err.WriteLine( "CharCmp.exe /fileA pathNameOfFileA /fileB pathNameOfFileB [/cp codePageNameOrNumber]" );
			err.WriteLine( "Example: CharCmp.exe /fileA .\\groceries.txt /fileB \\users\\home\\foo123\\groc.list.txt" );
			err.WriteLine( "Example: CharCmp.exe /fileA .\\letters\\to\\cleo.txt /fileB ..\\something.else /cp us-ascii" );
			err.WriteLine( "The default code-page is Windows-1252" );
		}
		private static void PrintUnkCodePage() {
			var err = System.Console.Error;
			err.WriteLine( "An unknown code page was specified.  Please try a different code page value." );
		}

	}

}