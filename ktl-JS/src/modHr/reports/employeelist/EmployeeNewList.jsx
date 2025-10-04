import React, { useState } from "react";
import { useGetData } from "../../../hooks/dataApi";
import { HashLoading } from "../../../components/Loading";
import Error from "../../../components/Error";
import { ListHeader, ListCol } from "../../../components/ListColWithHeader";
import PrintHeader from "../../../components/PrintHeader";
import SearchHeader from "../../../components/SearchHeader";
import { format } from "date-fns";
import PdfButton from "../../../components/button/PdfButton";

const EmployeeNewList = ({ dataForm }) => {
  const [query, setQuery] = useState("");
  const {
    data: list,
    error,
    isLoading,
    isError,
  } = useGetData(
    "employees",
    `/hrreports/new/${dataForm.fromDate}/${dataForm.tillDate}`
  );

  if (isLoading) return <HashLoading />;

  if (isError) return <Error message={error?.message} />;

  const data = list.data
    .filter((item) => {
      if (query === "") {
        return item;
      } else if (
        item.employeePin.toLowerCase().indexOf(query.toLowerCase()) !== -1 ||
        item.employeeName.toLowerCase().indexOf(query.toLowerCase()) !== -1 ||
        item.branchName.toLowerCase().indexOf(query.toLowerCase()) !== -1 ||
        item.departmentName.toLowerCase().indexOf(query.toLowerCase()) !== -1 ||
        item.designationName.toLowerCase().indexOf(query.toLowerCase()) !== -1
      ) {
        return item;
      } else return null;
    })
    .map(
      ({
        employeeId,
        branchName,
        departmentName,
        employeePin,
        employeeName,
        joiningDate,
        designationName,
        contactNumber,
        grossSalary,
        grossSalaryUsd,
      }) => ({
        employeeId,
        branchName,
        departmentName,
        employeePin,
        employeeName,
        joiningDate,
        designationName,
        contactNumber,
        grossSalary,
        grossSalaryUsd,
      })
    );

  return (
    <>
      <div className="flex justify-end items-center">
        <PdfButton
          path={`/HrPdfCommon/new/${dataForm.fromDate}/${dataForm.tillDate}`}
        />
        <PrintHeader
          fileName="EmployeeNew.csv"
          data={data.map(
            ({
              branchName,
              departmentName,
              employeePin,
              employeeName,
              joiningDate,
              designationName,
              contactNumber,
              grossSalary,
              grossSalaryUsd,
            }) => ({
              branchName,
              departmentName,
              employeePin,
              employeeName,
              joiningDate,
              designationName,
              contactNumber,
              grossSalary,
              grossSalaryUsd,
            })
          )}
          headers={[
            { label: "Branch Name", key: "branchName" },
            { label: "Department Name", key: "departmentName" },
            { label: "Employee Pin", key: "employeePin" },
            { label: "Employee Name", key: "employeeName" },
            { label: "Joining Date", key: "joiningDate" },
            { label: "Designation", key: "designationName" },
            { label: "Contact Number", key: "contactNumber" },
            { label: "Gross Salary", key: "grossSalary" },
            { label: "Gross Salary (USD)", key: "grossSalaryUsd" },
          ]}
        />
      </div>
      <SearchHeader
        action={setQuery}
        placeholder="PIN / Name / Designation / Department / Branch"
      />
      <div className="list-wrapper">
        <div className="md:grid grid-cols-9 list-header">
          <ListHeader label="Branch" />
          <ListHeader label="Department" />
          <ListHeader label="PIN" />
          <ListHeader label="Employee Name" />
          <ListHeader label="Joining Date" />
          <ListHeader label="Designation" />
          <ListHeader label="Contact Number" />
          <ListHeader label="Gross Salary" className="flex justify-end" />
          <ListHeader label="" />
        </div>
        {data.length > 0 &&
          data.map((item) => (
            <div
              key={item.employeeId}
              className="grid grid-cols-1 md:grid-cols-9 list-body"
            >
              <ListCol label="Branch:" value={item.branchName} />
              <ListCol label="Department: " value={item.departmentName} />
              <ListCol label="PIN : " value={item.employeePin} />
              <ListCol label="Employee Name: " value={item.employeeName} />
              <ListCol
                label="Joining Date: "
                value={format(new Date(item.joiningDate), "dd/MMM/yyyy")}
              />
              <ListCol label="Designation: " value={item.designationName} />
              <ListCol label="Contact Number: " value={item.contactNumber} />
              <ListCol
                label="Gross Salary : "
                value={
                  item.grossSalaryUsd === 0
                    ? item.grossSalary.toLocaleString("en-US") +
                      ` ${import.meta.env.VITE_CURRENCY}`
                    : item.grossSalaryUsd.toLocaleString("en-US") + " USD"
                }
                className="flex justify-start md:justify-end"
              />
              <div className="flex justify-end space-x-2">
                <PdfButton
                  //path={`/joiningletter?employeeid=${item.employeeId}`}
                  path={`/hrPdfCommon/JoiningLetter/${item.employeeId}`}
                  filename={`AppointmentToThePostOf${item.designationName}_${item.employeeId}.pdf`}
                  method="Get"
                  payload={item}
                />
              </div>
            </div>
          ))}

        <div className="list-footer">
          <div className="col-span-10"></div>
          <div className="flex justify-center">
            <span className="font-semibold">Total : {data.length}</span>
          </div>
        </div>
      </div>
    </>
  );
};

export default EmployeeNewList;
