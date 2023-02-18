<script>
  // StuffCreate.svelte
  import { navigate } from "svelte-navigator";
  import { crud } from "../lib/const.js";
  import { apiCreateStuffAsync } from "../lib/api.js";
  import CommonForm from "./CommonForm.svelte";

  let inputError = "";
  let stuffDatum = {
    id: "creating",
    label: "",
    description: "",
    otherInfo: "",
  };

  const handleSubmit = async () => {
    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiCreateStuffAsync(stuffDatum);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

<CommonForm
  title={crud.CREATE}
  {stuffDatum}
  {inputError}
  disabled={false}
  {handleSubmit}
/>
