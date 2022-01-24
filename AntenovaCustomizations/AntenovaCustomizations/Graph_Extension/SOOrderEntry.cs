using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PX.Objects.Common.Bql;
using PX.Objects;
using PX.Objects.SO;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.CR;

namespace PX.Objects.SO
{
    public class SOOrderEntry_Extension : PXGraphExtension<SOOrderEntry>
    {
        #region Event Handlers
        protected void SOOrder_CustomerID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
        {
            var row = (SOOrder)e.Row;
            if (row == null) return;

            Note CustomerNote = SelectFrom<Note>.
                                LeftJoin<BAccountR>.On<BAccountR.noteID.IsEqual<Note.noteID>>.
                                Where<BAccountR.bAccountID.IsEqual<P.AsInt>>.View.ReadOnly.Select((PXGraph)this.Base, row.CustomerID);

            if (CustomerNote != null && CustomerNote?.NotePopupText?.Length > 0)
                PXNoteAttribute.SetNote(Base.Document.Cache, Base.Document.Current, CustomerNote.NotePopupText);
            else
                PXNoteAttribute.SetNote(Base.Document.Cache, Base.Document.Current, "");
        }
        #endregion
    }
}