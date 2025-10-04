import { format } from "date-fns";
import { useGetData } from "../../hooks/dataApi";
import { HashLoading } from "../Loading";
import Error from "../Error";
import { MdOutlineDateRange } from "react-icons/md";
import { Link } from "react-router-dom";

const BusinessDateForMenu = () => {
  const {
    data: list,
    error,
    isLoading,
    isError,
    // refetch,
  } = useGetData("BusinessDay", "/acDay/list");

  if (isLoading) return <HashLoading />;

  if (isError) return <Error message={error.message} />;

  return (
    <Link
      className="flex items-center justify-center p-1 md:p-2 rounded-lg hover:text-orange "
      to="/settings/day/list"
    >
      <span
        className={`${
          list.data.status === "Day Open" ? "text-black" : "text-red-600"
        }`}
      >
        <MdOutlineDateRange size={30} />
      </span>
      <span
        className={`ml-1 text-lg font-bold hidden lg:block ${
          list.data.status === "Day Open" ? "text-black" : "text-red-600"
        }`}
      >
        {list.data.status === "Day Open" ? "Day Open : " : "Day Closed : "}
        {format(new Date(list.data.businessDate), "dd-MMM-yyyy")}
      </span>
    </Link>
  );
};

export default BusinessDateForMenu;
