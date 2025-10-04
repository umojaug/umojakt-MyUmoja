import Error from "../../../components/Error";
import { ListCol, ListColDetails } from "../../../components/ListColWithHeader";
import { HashLoading } from "../../../components/Loading";
import TopHeader from "../../../components/TopHeader";
import { useGetData } from "../../../hooks/dataApi";
import { format } from "date-fns";


const FeedbackDetails = ({ feedbackId }) => {
  const {
    data: list,
    error,
    isLoading,
    isError,
  } = useGetData("saleslead", `/myfeedback/Details/${feedbackId}`);

  if (isLoading) return <HashLoading />;

  if (isError) return <Error message={error.message} />;

  return (
    <>
      <TopHeader title={list.data.feedbackType + " Details"} />
      <div className="grid grid-cols-1 md:grid-cols-8 rounded-lg py-2 px-3 bg-gray-200 text-sm">
        <div className="col-span-2">
          <ListColDetails label="Branch : " value={list.data.branchName} />
          <ListColDetails
            label="Department : "
            value={list.data.departmentName}
          />
          <ListColDetails label="PIN : " value={list.data.employeePin} />
          <ListColDetails
            label="Employee Name : "
            value={list.data.employeeName}
          />
          <ListColDetails
            label="Designation : "
            value={list.data.designationName}
          />
          <ListColDetails
            label="Contact Number : "
            value={list.data.contactNumber}
          />
          <ListColDetails
            label="To Department : "
            value={list.data.departmentType}
          />
          <ListColDetails
            label="Entry Date: "
            value={format(new Date(list.data.entryDate), "dd/MMM/yyyy")}
          />
        </div>
        <ListCol
          label="Particulars : "
          value={list.data.particulars}
          className="md:col-span-6 whitespace-pre-line"
        />
      </div>
    </>
  );
};

export default FeedbackDetails;
