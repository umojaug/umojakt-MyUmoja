import React, { Fragment, useState } from "react";
import { AiOutlineLock } from "react-icons/ai";
import { usePostData } from "../../hooks/dataApi";

import toast from "react-hot-toast";
import DialogBody from "./DialogBody";

const MonthLockButton = ({ selectMonth, selectYear }) => {
  const { mutateAsync } = usePostData();
  const onSubmit = async () => {
    try {
      const { status } = await mutateAsync({
        path: "/emppayroll/monthstatus",
      });
      if (status === 201) {
        toast.success("Saved successfully!");
      }
    } catch (error) {
      if (error.response) {
        toast.error("Response : " + error.response.data);
      } else if (error.request) {
        toast.error("Request : " + error.message);
      } else {
        toast.error("Error :", error.message);
      }
    } finally {
      closeModal();
    }
  };

  let [isOpen, setIsOpen] = useState(false);

  function closeModal() {
    setIsOpen(false);
  }

  function openModal() {
    setIsOpen(true);
  }

  return (
    <>
      <button
        className="bg-gray-200 grid place-items-center p-2 hover:bg-primary hover:text-gray-300 rounded-lg"
        onClick={() => {
          openModal();
        }}
      >
        <AiOutlineLock size={40} />
        Close Month
      </button>
      <DialogBody
        closeModal={closeModal}
        isOpen={isOpen}
        onSubmit={onSubmit}
        bodyText="Did you complete your monthly reports? Are you sure, do you want to close the month?"
      />
    </>
  );
};

export default MonthLockButton;
