// store.js
import { writable } from "svelte/store";

// export const selectedItem = writable({});
export const selectedItem = writable(JSON.parse(localStorage.getItem("selectedItem")) || {});
selectedItem.subscribe(val => localStorage.setItem("selectedItem", JSON.stringify(val)));

export const tokens = writable(null);
tokens.subscribe(value => value);
