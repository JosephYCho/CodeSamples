import * as petService from "../service/petService";

export function getPetPersonal() {
  return dispatch => {
    return petService
      .getAllPetPersonal()
      .then(response => {
        dispatch(onGetPetPersonalSuccess(response.items));
      })
      .catch(error => {
        throw error;
      });
  };
}

export function onGetPetPersonalSuccess(pets) {
  console.log(pets);
  return {
    type: "GET_PERSONALPET",
    pets: pets
  };
}

export function postPetPersonal(data) {
  return dispatch => {
    return petService
      .insertPetPersonal(data)
      .then(response => console.log(response))
      .then(() => {
        dispatch(onInsertPetPersonalSuccess(data));
      });
  };
}

export function onInsertPetPersonalSuccess(data) {
  return {
    type: "POST_PERSONALPET",
    postedPets: data
  };
};

export function deletePet(id){
  return dispatch =>{
     return petService.deletePetPersonal(id)
     .then(response =>{ dispatch(onDeleteSuccess(response.item))})
  }
}

export function onDeleteSuccess(response){
  return{
    type:"DELETE_PERSONALPET",
    pets:response
  }
}

export function mapPets(pets) {
  return dispatch => {
    dispatch({ type: "MAP_PETS", petsTemplate: pets });
  };
}
