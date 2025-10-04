import React from "react";
import { Link } from "react-router-dom";
import Menu from "./Menu";

const Sidebar = () => {
  return (
    <div className="grid grid-rows-twoRows w-60 h-screen overflow-hidden">
      <div className="grid place-content-center shadow-lg">
        <Link to="/dashboard">
          <img className="h-8" src="/images/logo.jpg" alt="logo" />
        </Link>
      </div>
      <div className="overflow-y-auto">
        <div className="grid grid-cols-1 place-items-center p-2">
          <Menu />
        </div>
      </div>
    </div>
  );
};

export default Sidebar;
