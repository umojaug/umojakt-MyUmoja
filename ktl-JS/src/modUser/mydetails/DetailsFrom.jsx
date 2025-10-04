/* eslint-disable react/prop-types */
import { useState } from "react";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import toast from "react-hot-toast";
import { usePostData } from "../../hooks/dataApi";
import Input from "../../components/Input";
import { SelectFromDb, SelectFromOptions } from "../../components/SelectList";
import { selectOptions } from "../../data/selectOptions";

const schema = yup.object({
  // contactNumber: yup.string().max(50).required("Required"),
  // contactAddress: yup.string().max(50).required("Required"),
  // maritalStatus: yup.string().max(50).required("Required"),
  // email: yup.string().max(50).required("Required"),
  // educationId: yup.string().max(50).required("Required"),
});

const DetailsFrom = ({ defaultValues, closeModal, path, titleText }) => {
  const { mutateAsync } = usePostData();
  const [submitting, setSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm({
    defaultValues: defaultValues,
    resolver: yupResolver(schema),
  });
  const { contactAddress, contactNumber, maritalStatus, email, educationId } =
    errors;

  const onSubmit = async (formData) => {
    console.log(formData);
    var data = new FormData();
    data.append("employeeId", formData.employeeId);
    data.append("contactNumber", formData.contactNumber);
    data.append("contactAddress", formData.contactAddress);
    data.append("maritalStatus", formData.maritalStatus);
    data.append("email", formData.email);
    data.append("educationId", formData.educationId);
    // data.append("educationId", formData.educationId);


    try {
      const { status } = await mutateAsync({
        path: path,
        formData: data,
      });
      if (status === 201) {
        toast.success("Saved successfully!");
        reset();
        // addCreateNew === false && closeModal();
      }
      if (status === 204) {
        toast.success("Update successful!");
        closeModal();
        // navigate(returnPath);
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
      setSubmitting(false);
    }
  };

  return (
    <div className="w-full h-auto bg-white rounded-md shadow-lg p-6">
      <div className="w-1/3 pr-4 border-r border-gray-300">
        <h2 className="text-xl font-semibold text-gray-800 mb-4">
          {titleText}
        </h2>
      </div>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Input
          name="contactNumber"
          label="Contact Number"
          type="text"
          register={register}
          errorMessage={contactNumber?.message}
        />
        <Input
          name="email"
          label="Email"
          type="email"
          register={register}
          errorMessage={email?.message}
        />
        <SelectFromOptions
          register={register}
          options={selectOptions.maritalStatus}
          label="Marital status List"
          name="maritalStatus"
          errorMessage={maritalStatus?.message}
        />
        <Input
          name="contactAddress"
          label="Contact Address"
          type="text"
          register={register}
          errorMessage={contactAddress?.message}
        />
        <SelectFromDb
          control={control}
          label="Education"
          path="/educations/select"
          name="educationId"
          errorMessage={educationId?.message}
        />
        <div className="flex justify-end space-x-4 mt-4">
          <button
            type="submit"
            disabled={submitting}
            className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          >
            Update
          </button>
        </div>
      </form>
    </div>
  );
};

export default DetailsFrom;
