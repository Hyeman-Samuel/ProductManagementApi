using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Utility
{
    public class BaseController : ControllerBase
    {
        protected List<string> ListModelStateErrors
        {
            get
            {
                return ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            }
        }

        public IActionResult ApiResponse<T>(T data = default, string message = "",
            ApiResponseCode codes = ApiResponseCode.OK, int? totalCount = 0, params string[] errors)
        {
            var response = new ApiResponse<T>(data, message, codes, errors);
            response.Description = message ?? response.Code.GetDescription();
            return Ok(response);
        }

        public IActionResult Response<T>(T data = default, string message = "",
            ApiResponseCode codes = ApiResponseCode.OK, int? totalCount = 0, params string[] errors)
        {
            var response = new ApiResponse<T>(data, message, codes, errors);
            response.Description = message ?? response.Code.GetDescription();

            var statusCode = 500;
            switch (response.Code)
            {
                case ApiResponseCode.OK:
                    statusCode = 200;
                    break;
                case ApiResponseCode.NOTFOUND:
                    statusCode = 404;
                    break;
                case ApiResponseCode.INVALID_REQUEST:
                    statusCode = 400;
                    break;
                case ApiResponseCode.ERROR:
                    statusCode = 500;
                    break;
                default:
                    break;
            }

            return StatusCode(statusCode, response);
        }


    }
}
