let defaultState = {
  pets: [],
  postedPets: []
};

const mainReducer = (state = defaultState, action) => {
  if (action.type === "GET_PERSONALPET") {
    return {
      ...state,
      pets: action.pets
    };
  }
  if (action.type === "MAP_PETS") {
    return {
      ...state,
      petsTemplate: action.petsTemplate
    };
  }

  if (action.type === "POST_PERSONALPET") {
    return {
      ...state,
      postedPets: [...state.pets, action.postedPets]
    };
  } 

  if(action.type === "DELETE_PERSONALPET"){
    return{
      ...state,
      pets:state.pets.filter(pet=> pet.id !== action.pets)
    }
  }
  else {
    return {
      ...state
    };
  }
};

export default mainReducer;
