import React from "react";
import { connect } from "react-redux";
import * as actionCreators from "../../actions/index";
import MapPets from "./MapPets";


class PetContainer extends React.Component {
  componentDidMount() {
    this.props.getPetPersonal();
    console.log(this.props.pets);
  }
  onEditClick = pet => {
    this.props.history.push("/postyourpets", pet);
  };

  onDeleteClick = id => {
    console.log(id);
    this.props.deletePet(id)
    
  };

  render() {
    return (
      <div>
        <MapPets
          pets={this.props.pets}
          onEditClick={this.onEditClick}
          onDeleteClick={this.onDeleteClick}
        />
      </div>
    );
  }
}

const mapStatetoProps = state => {
  return state;
};

export default connect(
  mapStatetoProps,
  actionCreators
)(PetContainer);
