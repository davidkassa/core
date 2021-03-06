﻿using System;
using Core.Models.Data;
using Bit.Core.Models.Table;
using System.Collections.Generic;

namespace Bit.Core.Models.Api
{
    public class LoginResponseModel : ResponseModel
    {
        public LoginResponseModel(Cipher cipher, GlobalSettings globalSettings, bool orgUseTotp, string obj = "login")
            : base(obj)
        {
            if(cipher == null)
            {
                throw new ArgumentNullException(nameof(cipher));
            }

            if(cipher.Type != Enums.CipherType.Login)
            {
                throw new ArgumentException(nameof(cipher.Type));
            }

            var data = new LoginDataModel(cipher);

            Id = cipher.Id.ToString();
            OrganizationId = cipher.OrganizationId?.ToString();
            Name = data.Name;
            Uri = data.Uri;
            Username = data.Username;
            Password = data.Password;
            Notes = data.Notes;
            Totp = data.Totp;
            RevisionDate = cipher.RevisionDate;
            Edit = true;
            OrganizationUseTotp = orgUseTotp;
            Attachments = AttachmentResponseModel.FromCipher(cipher, globalSettings);
        }

        public LoginResponseModel(CipherDetails cipher, GlobalSettings globalSettings, string obj = "login")
            : this(cipher as Cipher, globalSettings, cipher.OrganizationUseTotp, obj)
        {
            FolderId = cipher.FolderId?.ToString();
            Favorite = cipher.Favorite;
            Edit = cipher.Edit;
        }

        public string Id { get; set; }
        public string OrganizationId { get; set; }
        public string FolderId { get; set; }
        public bool Favorite { get; set; }
        public bool Edit { get; set; }
        public bool OrganizationUseTotp { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
        public string Totp { get; set; }
        public IEnumerable<AttachmentResponseModel> Attachments { get; set; }
        public DateTime RevisionDate { get; set; }
    }
}
