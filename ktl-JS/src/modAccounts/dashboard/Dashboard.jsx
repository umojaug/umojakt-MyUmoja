import React from "react";
import TopHeader from "../../components/TopHeader";
import BusinessDay from "../settings/dayOpenClose/BusinessDay";

const Dashboard = () => {
  return (
    <div className="card w-full max-w-screen-xl">
      <TopHeader title="Accounts Dashboard" />
      <BusinessDay />
    </div>
  );
};

export default Dashboard;
