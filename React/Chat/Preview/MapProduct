import React from "react";
import MapProductSolo from "./MapProductSolo";
import PropTypes from "prop-types";

const MapProducts = ({ products,url,checkForHttp }) => {
  const mapProducts = products.map((product,index)=>(
    <MapProductSolo
      product={product}
      index={index}
      url={url[index]}
      checkForHttp={checkForHttp}
    />
    );

  );
  return mapProducts;
};

MapProducts.propTypes = {
  product: PropTypes.object,
  index: PropTypes.number,
  checkForHttp: PropTypes.func,
  url: PropTypes.string
};

export default React.memo(MapProducts);
