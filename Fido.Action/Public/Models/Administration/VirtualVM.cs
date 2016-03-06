using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using AutoMapper;
using Fido.Core;
using Fido.Service;
using Fido.Action.Implementation;

// http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx

namespace Fido.Action.Models
{
    // No nested rows... how do I enforce this?

    public interface IElement
    {
        void Validate();
        string Html();
    }

    public class Row : IElement
    {
        public int Sections { get; private set; }
        public IList<Field> Fields { get; private set; }

        public Row(int Sections)
        {
            if (Sections < 1 || Sections > 12)
                throw new Exception("A row must contain 1 to 12 sections");

            this.Sections = Sections;
            this.Fields = new List<Field>();
        }

        #region Validate
        public void Validate()
        {
            Check();

            foreach(IElement Field in Fields)
                Field.Validate();
        }

        private void Check()
        {
            int TotalFieldSpan = 0;

            foreach (Field Field in Fields) TotalFieldSpan += Field.Span;

            if (TotalFieldSpan != Sections)
                throw new Exception("Field span to row section mismatch");
        }
        #endregion
    
        public string Html()
        {
            var Html = new StringBuilder();
            Html.Append(string.Concat("<div data-row-span=\"", Sections.ToString(), "\">\n"));

            foreach (IElement Field in Fields)
                Html.Append(Field.Html());

            Html.Append("</div>\n");
            return Html.ToString();
        }
    }

    public class FieldSet : IElement
    {
        // can contain; other fieldsets, fields or rows
        public string Legend { get; private set; }
        public IList<IElement> Elements { get; set; }

        public FieldSet()
        {
            this.Legend = String.Empty;
            this.Elements = new List<IElement>();
        }

        public FieldSet(string Legend)
        {
            this.Legend = Legend;
            this.Elements = new List<IElement>();
        }

        #region Validate
        public void Validate()
        {
            foreach (IElement Element in Elements)
                Element.Validate();
        }
        #endregion

        public string Html()
        {
           var Html = new StringBuilder();
            Html.Append("<fieldset>\n");

            if (Legend != string.Empty)
                Html.Append(string.Concat("<legend>", Legend, "</legend>\n"));

            foreach (IElement Element in Elements)
                Html.Append(Element.Html());

            Html.Append("</fieldset>\n");
            return Html.ToString();
        }
    }

    public abstract class Field : IElement
    {
        public int Span { get; private set; }

        public Field(int Span)
        {
            this.Span = Span;
        }

        public string Html()
        {
            var Html = new StringBuilder();

            Html.Append(string.Concat("<div data-field-span=\"", Span.ToString(), ">\n"));
            Html.Append(InnerHtml());
            Html.Append("</div>\n");

            return Html.ToString();
        }

        public abstract void Validate();
        protected abstract string InnerHtml();
    }

    //public class SelectList : Field
    //{
    //    public SelectList(int Span, Guid ListId)
    //        : base(Span)
    //    {
    //    }

    //    public void Validate() { }
    //    public string InnerHtml() { return ""; }
    //}

    // CheckBox

    // RadioButtonGroup

    // RadioButton

    public enum TextBoxType
    {
        SingleLine = 0,
        MutiLine
    }

    public class TextBox : Field
    {
        private TextBoxType TextBoxType;
        private string Label = string.Empty;

        public TextBox(int Span, TextBoxType TextBoxType, string Label)
            : base(Span)
        {
            this.TextBoxType = TextBoxType;
            this.Label = Label;
        }

        public override void Validate() { }

        protected override string InnerHtml()
        {
            var Html = new StringBuilder();

            if (Label != string.Empty)
                Html.Append(String.Concat("<label>", Label, "</label>\n"));

            if (TextBoxType == TextBoxType.SingleLine)
                Html.Append("<input type=\"text\">");
            else
                Html.Append("<input type=\"textarea\">");

            return Html.ToString();
        }
    }

    public class VirtualVM : Model<VirtualVM>
    {
        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Data
        public Guid Id { get; set; } // Form Id

        public IList<IElement> Root = new List<IElement>();
        
        //[Display(Name = "firstname")]
        //[Required(ErrorMessage = "The first name field cannot be left blank")]
        //public string Firstname { get; set; }

        [Display(Name = "created date")]
        public DateTime CreatedUtc { get; set; }
        [Display(Name = "record age")]
        public int? CreatedAgeDays { get; set; }
        public bool IsNew { get; set; }
        public byte[] RowVersion { get; set; }
        #endregion

        public VirtualVM() { }
        public VirtualVM(
            IFeedbackAPI FeedbackAPI,
            IAuthenticationAPI LoginAPI,
            IModelAPI ModelAPI)
                : base (FeedbackAPI, LoginAPI, ModelAPI,
                        RequiresReadPermission: true,
                        RequiresWritePermission: true)
        { }

        public override VirtualVM Prepare(VirtualVM Model)
        {
            using (new FunctionLogger(Log))
            {
                return new VirtualVM(); // TO DO
            }
        }

        public override VirtualVM Read(Guid Id)
        {
            using (new FunctionLogger(Log))
            {
                // TO DO: I think I might need 2 components to the model - the UI description and the data collection!!!
                var Model = LoadVirtualModel(Id);
                Model = PopulateVirtualModel(new Guid()); // How do I get the Id for the record?

                return Model;
            }
        }

        private VirtualVM LoadVirtualModel(Guid VMId)
        {
            // Below implments this...
            //    <fieldset>
            //      <legend>Please open an account at</legend>
            //      <div data-row-span="1">
            //        <div data-field-span="1">
            //            <label>Branch Name</label>
            //            <input type="text" autofocus>
            //        </div>
            //      </div>
            //    </fieldset>

            // TO DO...
            VirtualVM Model = new VirtualVM();

            FieldSet fs = new FieldSet("Please open an account at");
            Row rw = new Row(1);
            var tb = new TextBox(1, TextBoxType.SingleLine, "Branch Name");
            rw.Fields.Add(tb);
            fs.Elements.Add(rw);

            rw.Validate();

            Model.Root.Add(fs);

            return Model;
        }

        private VirtualVM PopulateVirtualModel(Guid RecId)
        {
            return new VirtualVM(); // TO DO
        }

        public override bool Save(VirtualVM Model)
        {
            using (new FunctionLogger(Log))
            {
                FeedbackAPI.DisplaySuccess("The details have been saved");
                return true;
            }
        }

        public override bool Delete(VirtualVM Model)
        {
            using (new FunctionLogger(Log))
            {
                FeedbackAPI.DisplaySuccess("The details have been deleted");
                return true;
            }
        }
    }
}
