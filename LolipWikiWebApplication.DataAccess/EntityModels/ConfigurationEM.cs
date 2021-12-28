using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LolipWikiWebApplication.DataAccess.Models;

namespace LolipWikiWebApplication.DataAccess.EntityModels
{
    [Table("TAB_CONFIGURATION")]
    public class ConfigurationEM
    {
        protected ConfigurationEM(string readArticleControlType, string writeArticleControlType)
        {
            ReadArticleControlType  = readArticleControlType;
            WriteArticleControlType = writeArticleControlType;
        }

        public ConfigurationEM(AccessControlType readArticleControlType, AccessControlType writeArticleControlType) : this(readArticleControlType.ToString(), writeArticleControlType.ToString())
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string ReadArticleControlType { get; set; }

        [Required]
        [StringLength(int.MaxValue)]
        public string WriteArticleControlType { get; set; }

#region NotMapped

        [NotMapped]
        public AccessControlType ReadArticleControlTypeEnum
        {
            get => Enum.Parse<AccessControlType>(ReadArticleControlType);
            set => ReadArticleControlType = value.ToString();
        }

        [NotMapped]
        public AccessControlType WriteArticleControlTypeEnum
        {
            get => Enum.Parse<AccessControlType>(WriteArticleControlType);
            set => WriteArticleControlType = value.ToString();
        }

#endregion
    }
}