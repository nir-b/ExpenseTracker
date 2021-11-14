import React from 'react';

import 'devextreme/data/odata/store';
import {
  Column,
  DataGrid,
  FilterRow,
  HeaderFilter,
  GroupPanel, 
  Editing, 
  Summary, 
  RequiredRule,
  StringLengthRule, 
  TotalItem, 
  Export
} from 'devextreme-react/data-grid';
import { createStore } from 'devextreme-aspnet-data-nojquery';
import { Workbook } from 'exceljs';
import saveAs from 'file-saver';
import { exportDataGrid } from 'devextreme/excel_exporter';

const url = 'http://localhost:5000/expenses';

const dataSource = createStore({
  key: 'id',
  loadUrl: `${url}/GetExpenses`,
  insertUrl: `${url}/AddExpense`,
  updateUrl: `${url}/UpdateExpense`,
  deleteUrl: `${url}/DeleteExpense`
});

function exportGrid(e) {
  const workbook = new Workbook(); 
  const worksheet = workbook.addWorksheet("Expenses"); 
  exportDataGrid({ 
      worksheet: worksheet, 
      component: e.component
  }).then(function() {
      workbook.xlsx.writeBuffer().then(function(buffer) { 
          saveAs(new Blob([buffer], { type: "application/octet-stream" }), "Expenses.xlsx"); 
      }); 
  });
  e.cancel = true; 
}

const App = (props) => {
  const expenseAmountFormat = { style: 'currency', currency: 'CHF', useGrouping: true};

  return (
    <DataGrid
      dataSource={dataSource}
      showBorders={true}      
      height={1000}
      remoteOperations={false}
      onExporting={exportGrid}>
      <FilterRow visible={true} />
      <HeaderFilter visible={true} />
      <GroupPanel visible={false} />
      
      <Editing mode="row" allowAdding={true} allowDeleting={true} allowUpdating={true}/>     
      <Export enabled={true} />

      <Column dataField="title" caption="Title">
        <RequiredRule message="Please enter a Title"/>
        <StringLengthRule max={512} message="The Expense title must be a string with a maximum length of 512." />
      </Column>

      <Column dataField="expenseDate" dataType="date" caption="Date" defaultSortIndex={0} defaultSortOrder="desc">
        <RequiredRule message="Please enter an expense date." />
      </Column>

      <Column dataField="category" caption="Category">
        <RequiredRule message="Please enter an expense category." />
        <StringLengthRule max={128} message="The Expense category must be a string with a maximum length of 128." />
      </Column>

      <Column dataField="amount">
        <RequiredRule message="The Expense amount field is required." />        
      </Column>

      <Column dataField="description">
        <StringLengthRule max={1024} message="The field Desctiption must be a string with a maximum length of 1024" />
      </Column>
      <Summary>
        <TotalItem column="amount" summaryType="sum" showInColumn="amount" valueFormat={expenseAmountFormat} displayFormat="Total: {0}">         
         
        </TotalItem>
        <TotalItem column="title" summaryType="count" showInColumn="title" displayFormat="Expense Items: {0}"/>   
      </Summary>
    </DataGrid>
  );
}

export default App;
