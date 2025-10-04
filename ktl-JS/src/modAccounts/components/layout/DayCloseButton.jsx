import React, { useState } from "react";

import toast from "react-hot-toast";
import { usePostData } from "../../../hooks/dataApi";
import DialogBody from "../../../components/button/DialogBody";

const DayCloseButton = ({ path, action }) => {
  const { mutateAsync } = usePostData();
  const onSubmit = async () => {
    try {
      const { status } = await mutateAsync({ path });

      if (status === 204) {
        toast.success("Successfully InActive!");
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
      action();
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
        className=" btn bg-red-600 w-20  font-bold"
        onClick={() => {
          openModal();
        }}
        data-tooltip-id="my-tooltip"
        data-tooltip-content="DayClose"
      >
        {" "}
        Day close
      </button>

      <DialogBody
        closeModal={closeModal}
        isOpen={isOpen}
        onSubmit={onSubmit}
        bodyText="Are you sure you want to Day Close?"
      />
    </>
  );
};

export default DayCloseButton;
