using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Myapp.Models.Domain;
using Myapp.Models.Requests;
using Myapp.Services;
using Myapp.Web.Controllers;
using Myapp.Web.Models.Responses;
using System;
using System.Collections.Generic;


namespace Myapp.Web.Api.Controllers
{
    [Route("api/metaTags")]
    [ApiController]
    public class MetaTagApiController : BaseApiController
    {
        private IMetaTagService _metaDataService;
        private readonly IAuthenticationService<int> _authenticationService;

        public MetaTagApiController(IMetaTagService metaDataService, IAuthenticationService<int> authenticationService, ILogger<MetaTagApiController> logger) : base(logger)
        {
            _metaDataService = metaDataService;
            _authenticationService = authenticationService;
        }

        [HttpGet]
        public ActionResult<ItemsResponse<MetaTag>> GetAll()
        {
            ItemsResponse<MetaTag> response = null;
            ActionResult result = null;

            try
            {
                List<MetaTag> metaTags = _metaDataService.Select();

                if (metaTags == null)
                {
                    result = NotFound404(new ErrorResponse("Not Found"));
                }
                else
                {
                    response = new ItemsResponse<MetaTag>();
                    response.Items = metaTags;

                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<MetaTag>> SelectById(int id)
        {
            ItemResponse<MetaTag> response = null;
            ActionResult result = null;
            try
            {
                MetaTag metaTag = _metaDataService.Select(id);
                if (metaTag == null)
                {
                    result = NotFound404(new ErrorResponse("You put the wrong id. Nothing exist with that id"));
                }
                else
                {
                    response = new ItemResponse<MetaTag>();

                    response.Item = metaTag;
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Insert(MetaTagCreateRequest req)
        {
            ItemResponse<int> response = null;
            ActionResult result = null;
            int currentUserId = _authenticationService.GetCurrentUserId();
            try
            {
                int id = _metaDataService.Insert(req, currentUserId);
                if (id > 0)
                {
                    response = new ItemResponse<int>();
                    response.Item = id;
                    result = Ok200(response);
                }
                else
                {
                    result = NotFound404(new ErrorResponse("Error"));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }

            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(MetaTagUpdateRequest req)
        {
            SuccessResponse response = null;
            ActionResult result = null;

            try
            {
                _metaDataService.Update(req);
                response = new SuccessResponse();
                result = Ok200(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            SuccessResponse response = null;
            ActionResult result = null;
            try
            {
                _metaDataService.Delete(id);
                response = new SuccessResponse();
                result = Ok200(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
    }
}
