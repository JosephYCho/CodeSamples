
const isProdUrl = message => {
  const url = message.match(
    /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._~#=]{2,256}\/addToCart\/\d+\/\d+/g
  );
  return url;
};

const isInfluencerUrl = message => {
  const url = message.match(
    /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._~#=]{2,256}\/influencers\/userId\W\d+\S/g
  );
  return url;
};

const isOtherUrl = message => {
  const otherUrl = message.match(
    /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_.~#?&//=]*)/g
  );
  return otherUrl;
};

const isWebUrl = message => {
  const webUrl = message.match(
    /(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_.~#?&//=]*)/g
  );
  return webUrl;
};

const addAnchor = message => {
  let newMessage = message
    .replace(
      /(?:^|[^"'])((ftp|http|https|file:):(\/\/[\S]+(\b|$)))/gim,
      '<a href="http://$3" class="one" target="_blank">$&</a>'
    )
    .replace(
      /(www.[^ <]+(\b|$))/gim,
      '<a href="http://$1" class="two" target="_blank">$1</a>'
    );

  return newMessage;
};


export { isProdUrl, isInfluencerUrl, isOtherUrl, addAnchor, isWebUrl };
