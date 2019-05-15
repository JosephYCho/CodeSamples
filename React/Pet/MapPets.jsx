import React from "react";
import PersonalPetPureComponent from "./PersonalPetPureComponent";

const mapPets = ({ pets, onEditClick, onDeleteClick }) => {
  const mapPets = pets.map(pet => (
    <PersonalPetPureComponent
      key={pet.id}
      img={pet.imageUrl}
      title={pet.title}
      location={pet.location}
      description={pet.description}
      email={pet.email}
      onEditClick={onEditClick}
      onDeleteClick={onDeleteClick}
      pet={pet}
    />
  ));

  return mapPets;
};

export default React.memo(mapPets);
