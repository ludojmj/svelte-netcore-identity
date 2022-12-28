<script>
  // StuffCreate.svelte
  import { navigate } from "svelte-navigator";
  import { apiCreateStuff } from "../api/stuff";
  import { accessToken, idToken } from "../oidc/components.module"; // "@dopry/svelte-oidc";
  import CommonForm from "./CommonForm.svelte";

  let inputError = "";
  let stuffDatum = {
    id: "creating",
    label: "",
    description: "",
    otherInfo: "",
  };

  const handleChange = (event) => {
    inputError = "";
    const { name, value } = event.target;
    stuffDatum = { ...stuffDatum, [name]: value };
  };

  const handleSubmit = async (event) => {
    if (!/\S/.test(stuffDatum.label)) {
      inputError = "The label cannot be empty.";
      setTimeout(() => {
        inputError = "";
      }, 2000);

      return;
    }

    stuffDatum = await apiCreateStuff(stuffDatum, $accessToken, $idToken);
    if (!stuffDatum.error) {
      navigate("/");
    }
  };
</script>

<main>
  <CommonForm
    title="Creating a stuff"
    {stuffDatum}
    {inputError}
    readonly={false}
    {handleChange}
    {handleSubmit}
  />
</main>
