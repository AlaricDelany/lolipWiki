using System.ComponentModel.DataAnnotations;
using LolipWikiWebApplication.BusinessLogic.Logic;
using LolipWikiWebApplication.DataAccess;
using LolipWikiWebApplication.DataAccess.Models;
using LolipWikiWebApplication.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace LolipWikiWebApplication.Pages.Admin
{
    public class IndexModel : BasePageModel
    {
        private readonly IConfigurationLogic _configurationLogic;

        public IndexModel(
            ILolipWikiDbContext  dbContext,
            IUserManagementLogic userManagementLogic,
            IAccessControlLogic  accessControlLogic,
            IConfigurationLogic  configurationLogic
        ) : base(dbContext,
                 userManagementLogic,
                 accessControlLogic,
                 true
                )
        {
            _configurationLogic = configurationLogic;
        }

        [Required]
        [Display(Name = "Write Access")]
        [BindProperty]
        public AccessControlType WriteControlType { get; set; }

        [Required]
        [Display(Name = "Read Access")]
        [BindProperty]
        public AccessControlType ReadControlType { get; set; }

        public void OnGet()
        {
            ReadControlType  = _configurationLogic.GetReadControlType(DbContext, Requestor);
            WriteControlType = _configurationLogic.GetWriteControlType(DbContext, Requestor);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            _configurationLogic.Update(DbContext,
                                       Requestor,
                                       ReadControlType,
                                       WriteControlType
                                      );

            return RedirectToPage("/Index");
        }
    }
}