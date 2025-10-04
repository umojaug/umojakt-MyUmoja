import MonthPreviousButton from "./MonthPreviousButton";

const MonthPrevious = () => {
  return (
    <div className="card w-full max-w-screen-xl">
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        <MonthPreviousButton />
      </div>
    </div>
  );
};

export default MonthPrevious;
