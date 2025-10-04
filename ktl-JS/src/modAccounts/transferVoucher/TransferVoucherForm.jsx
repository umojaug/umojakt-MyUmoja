import { yupResolver } from "@hookform/resolvers/yup";
import React, { useState } from "react";
import { useForm } from "react-hook-form";
import toast from "react-hot-toast";
import * as yup from "yup";
import Input from "../../components/Input";
import { SelectFromDb } from "../../components/SelectList";
import TextArea from "../../components/TextArea";
import SaveButton from "../../components/button/SaveButton";
import { usePostData } from "../../hooks/dataApi";

const schema = yup.object({
  bankOrCashId: yup
    .number()
    .min(0, "Must be greater than or equal to 0")
    .typeError("Positive number required"),
  ledgerNameCode: yup.string().max(50).required("Required"),
  amount: yup
    .number()
    .min(0, "Must be greater than or equal to 0")
    .typeError("Positive number required")
    .transform((o, v) => parseInt(v.replace(/,/g, ""))),
  particulars: yup.string().max(250).required("Required"),
});

const TransferVoucherForm = ({
  defaultValues,
  path,
  selectPath,
  action,
  btnText,
}) => {
  const [submitting, setSubmitting] = useState(false);
  const [inputNumber, setInputNumber] = useState("");
  const { mutateAsync } = usePostData();
  const {
    register,
    handleSubmit,
    reset,
    control,
    formState: { errors },
  } = useForm({
    defaultValues: defaultValues,
    resolver: yupResolver(schema),
  });
  const { bankOrCashId, ledgerNameCode, amount, particulars } = errors;

  const onSubmit = async (formData) => {
    setSubmitting(true);
    var data = new FormData();
    data.append("bankOrCashId", formData.bankOrCashId);
    data.append("ledgerNameCode", formData.ledgerNameCode);
    data.append("amount", formData.amount);
    data.append("particulars", formData.particulars);
    try {
      const { status } = await mutateAsync({
        path: path,
        formData: data,
      });
      if (status === 201) {
        toast.success("Saved successfully!");
        reset();
      }
      setInputNumber("");
      if (status === 204) {
        toast.success("Update successful!");
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
      action();
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className="form-col">
        <SelectFromDb
          control={control}
          label="Select Bank Or Cash"
          path={selectPath}
          name="bankOrCashId"
          errorMessage={bankOrCashId?.message}
        />
        <SelectFromDb
          control={control}
          label="Select Account Head"
          path="/acLedger/selectByTransfer"
          name="ledgerNameCode"
          errorMessage={ledgerNameCode?.message}
        />

        <Input
          name="amount"
          label="Amount"
          type="text"
          register={register}
          errorMessage={amount?.message}
          inputNumber={inputNumber}
          setInputNumber={setInputNumber}
        />
        <TextArea
          control={control}
          name="particulars"
          label="Particulars"
          type="text"
          errorMessage={particulars?.message}
        />

        <SaveButton btnText={btnText} disabled={submitting} />
      </div>
    </form>
  );
};

export default TransferVoucherForm;
