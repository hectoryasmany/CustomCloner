using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CustomCloner.Module.BusinessObjects
{
    [DefaultClassOptions]
     public class Invoice : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public Invoice(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
        }

        bool copied;
        bool makeCopy;
        string description;
        string code;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        public bool MakeCopy
        {
            get => makeCopy;
            set => SetPropertyValue(nameof(MakeCopy), ref makeCopy, value);
        }
        [Browsable(false)]
        public bool Copied
        {
            get => copied;
            set => SetPropertyValue(nameof(Copied), ref copied, value);
        }
        protected override void OnSaving()
        {
            base.OnSaving();
           
        }
        protected override void OnSaved()
        {
            
            base.OnSaved();
            var current = Session.FindObject<Invoice>(new BinaryOperator("Oid", this.Oid, BinaryOperatorType.Equal));
            IObjectSpace os = XPObjectSpace.FindObjectSpaceByObject(current);
            if (current!=null)
            {
                if (!current.Copied && current.MakeCopy)
                {
                    current.Copied = true;
                    CloneHelper cloner = new CloneHelper(((XPObjectSpace)os).Session);
                    Invoice clonedInvoice = cloner.Clone(((XPObjectSpace)os).GetObject(current));
                    clonedInvoice.Copied = true;
                    os.CommitChanges();
                    os.ReloadObject(clonedInvoice);
                    Session.Reload(current);


                }
            }
           

        }
    }
}