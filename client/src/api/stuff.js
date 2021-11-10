// stuffList.js
import axios from "axios";

const rootApi = isProd ? "https://localhost:5001/api/stuff" : "http://localhost:3000/mock/stuff";
const isMock = rootApi.indexOf("mock") > -1;

export const apiGetStuffList = async (accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: rootApi + mock,
    data: {}
  };

  try {
    const result = await axios(getMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiSearchStuff = async (search, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}${mock}?search=${search}`,
    data: {}
  };

  try {
    const result = await axios(getMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiGotoPage = async (page, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}${mock}?page=${page}`,
    data: {}
  };

  try {
    const result = await axios(getMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiGetStuffById = async (id, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const getMsg = {
    method: "get",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`,
    data: {}
  };

  try {
    const result = await axios(getMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiCreateStuff = async (input, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const postMsg = {
    method: isMock ? "get" : "post",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: rootApi + mock,
    data: input
  };

  try {
    const result = await axios(postMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiUpdateStuff = async (id, input, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const putMsg = {
    method: isMock ? "get" : "put",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`,
    data: input
  };

  try {
    const result = await axios(putMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

export const apiDeleteStuff = async (id, accessToken, idToken) => {
  const mock = isMock ? ".json" : "";
  const deleteMsg = {
    method: isMock ? "get" : "delete",
    headers: { "authorization": `Bearer ${accessToken}`, "id_token": idToken },
    url: `${rootApi}/${id}${mock}`,
    data: {}
  };

  try {
    const result = await axios(deleteMsg);
    return result.data;
  } catch (error) {
    return { error: getErrorMsg(error) };
  }
};

const getErrorMsg = error => {
  const msg = "An error occured. Try again later.";
  if (error.response && error.response.data && error.response.data.error) {
    return error.response.data.error;
  }
  if (error.message) {
    return error.message;
  }
  if (error) {
    return error;
  }
  return msg;
};