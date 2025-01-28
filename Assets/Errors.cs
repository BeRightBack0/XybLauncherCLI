using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Its not being used by anything for now 

namespace XybLauncher.Assets
{
    public class Errors
    {
        public static class ErrorCodes
        {
            // General Errors (1-99)
            public const int SUCCESS = 0;
            public const int UNKNOWN_ERROR = 1;
            public const int INVALID_INPUT = 2;
            public const int NOT_INITIALIZED = 3;
            public const int OPERATION_FAILED = 4;

            // File Operations (100-199)
            public const int FILE_NOT_FOUND = 100;
            public const int FILE_ACCESS_DENIED = 101;
            public const int FILE_ALREADY_EXISTS = 102;
            public const int INVALID_FILE_FORMAT = 103;
            public const int FILE_READ_ERROR = 104;
            public const int FILE_WRITE_ERROR = 105;
            public const int DIR_NOT_FOUND = 106;

            // Database Operations (200-299)
            public const int DB_CONNECTION_FAILED = 200;
            public const int DB_QUERY_FAILED = 201;
            public const int DB_RECORD_NOT_FOUND = 202;
            public const int DB_DUPLICATE_ENTRY = 203;
            public const int DB_TRANSACTION_FAILED = 204;

            // Network Operations (300-399)
            public const int NETWORK_CONNECTION_FAILED = 300;
            public const int REQUEST_TIMEOUT = 301;
            public const int INVALID_RESPONSE = 302;
            public const int SERVER_ERROR = 303;
            // HTTP Status Related (310-329)
            public const int HTTP_BAD_REQUEST = 310;         // 400
            public const int HTTP_UNAUTHORIZED = 311;        // 401
            public const int HTTP_FORBIDDEN = 312;          // 403
            public const int HTTP_NOT_FOUND = 313;          // 404
            public const int HTTP_SERVER_ERROR = 314;       // 500
            public const int HTTP_SERVICE_UNAVAILABLE = 315; // 503
                                                             // Connection Issues (330-349)
            public const int DNS_RESOLUTION_FAILED = 330;
            public const int HOST_UNREACHABLE = 331;
            public const int PORT_UNREACHABLE = 332;
            public const int CONNECTION_REFUSED = 333;
            public const int SSL_CERTIFICATE_ERROR = 334;
            public const int PROXY_CONNECTION_FAILED = 335;
            // Data Transfer (350-369)
            public const int UPLOAD_FAILED = 350;
            public const int DOWNLOAD_FAILED = 351;
            public const int INCOMPLETE_RESPONSE = 352;
            public const int DATA_TRANSFER_INTERRUPTED = 353;
            public const int BANDWIDTH_LIMIT_EXCEEDED = 354;
            // Protocol Errors (370-389)
            public const int PROTOCOL_ERROR = 370;
            public const int INVALID_URL = 371;
            public const int UNSUPPORTED_PROTOCOL = 372;
            public const int TOO_MANY_REDIRECTS = 373;
            public const int WEBSOCKET_ERROR = 374;

            // Authentication & Authorization (400-499)
            public const int INVALID_CREDENTIALS = 400;
            public const int SESSION_EXPIRED = 401;
            public const int UNAUTHORIZED_ACCESS = 402;
            public const int ACCOUNT_LOCKED = 403;

            // BuildDownloader Specific Errors (500-599)
            public const int DOWNLOAD_URL_NOT_PROVIDED = 500;
            public const int DOWNLOAD_INTERRUPTED = 502;
            public const int INVALID_ZIP_FILE = 503;
            public const int EXTRACTION_FAILED = 504;
            public const int ZIP_DELETION_FAILED = 505;
            public const int INVALID_BUILD_PATH = 506;


            // Helper method to get error message
            public static string GetErrorMessage(int errorCode)
            {
                return errorCode switch
                {
                    SUCCESS => "Operation completed successfully",
                    UNKNOWN_ERROR => "An unknown error occurred",
                    INVALID_INPUT => "Invalid input provided",
                    NOT_INITIALIZED => "System not properly initialized",
                    OPERATION_FAILED => "Operation failed to complete",
                    FILE_NOT_FOUND => "Specified file could not be found",
                    FILE_ACCESS_DENIED => "Access to the file was denied",
                    FILE_ALREADY_EXISTS => "File already exists",
                    INVALID_FILE_FORMAT => "Invalid file format",
                    FILE_READ_ERROR => "Error reading from file",
                    FILE_WRITE_ERROR => "Error writing to file",
                    DB_CONNECTION_FAILED => "Database connection failed",
                    DB_QUERY_FAILED => "Database query execution failed",
                    DB_RECORD_NOT_FOUND => "Database record not found",
                    DB_DUPLICATE_ENTRY => "Duplicate database entry",
                    DB_TRANSACTION_FAILED => "Database transaction failed",
                    DIR_NOT_FOUND => "Directory Not Found",

                    // Network Errors
                    NETWORK_CONNECTION_FAILED => "Network connection failed",
                    REQUEST_TIMEOUT => "Request timed out",
                    INVALID_RESPONSE => "Invalid response received",
                    SERVER_ERROR => "Server error occurred",
                    // HTTP Status
                    HTTP_BAD_REQUEST => "Bad request - The server cannot process the request",
                    HTTP_UNAUTHORIZED => "Unauthorized - Authentication is required",
                    HTTP_FORBIDDEN => "Forbidden - Server refuses to authorize",
                    HTTP_NOT_FOUND => "Resource not found on the server",
                    HTTP_SERVER_ERROR => "Internal server error occurred",
                    HTTP_SERVICE_UNAVAILABLE => "Service temporarily unavailable",
                    // Connection Issues
                    DNS_RESOLUTION_FAILED => "Failed to resolve domain name",
                    HOST_UNREACHABLE => "Host is unreachable",
                    PORT_UNREACHABLE => "Network port is unreachable",
                    CONNECTION_REFUSED => "Connection was refused by the server",
                    SSL_CERTIFICATE_ERROR => "SSL/TLS certificate validation failed",
                    PROXY_CONNECTION_FAILED => "Failed to connect through proxy",
                    // Data Transfer
                    UPLOAD_FAILED => "Failed to upload data to server",
                    INCOMPLETE_RESPONSE => "Received incomplete response from server",
                    DATA_TRANSFER_INTERRUPTED => "Data transfer was interrupted",
                    BANDWIDTH_LIMIT_EXCEEDED => "Bandwidth limit has been exceeded",
                    // Protocol Errors
                    PROTOCOL_ERROR => "Network protocol error occurred",
                    INVALID_URL => "Invalid URL format or structure",
                    UNSUPPORTED_PROTOCOL => "Unsupported network protocol",
                    TOO_MANY_REDIRECTS => "Too many redirects encountered",
                    WEBSOCKET_ERROR => "WebSocket connection error",



                    INVALID_CREDENTIALS => "Invalid credentials provided",
                    SESSION_EXPIRED => "Session has expired",
                    UNAUTHORIZED_ACCESS => "Unauthorized access attempt",
                    ACCOUNT_LOCKED => "Account is locked",
                    _ => $"Undefined error code: {errorCode}"
                };
            }
        }


        // return new LoginResult(ErrorCodes.ACCOUNT_LOCKED); Example ok ok ok 



    }
}
